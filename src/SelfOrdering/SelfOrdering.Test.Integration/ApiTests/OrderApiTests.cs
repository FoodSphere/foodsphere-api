namespace FoodSphere.SelfOrdering.Test.Integration;

public class OrderApiTests(SharedAppFixture fixture, ITestOutputHelper output) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Order_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var branch = await builder.SeedBranch(restaurant);
        var table = await builder.SeedTable(branch);
        var bill = await builder.SeedBill(table);
        var member = await builder.SeedBillMember(bill);

        var items = new List<OrderItemRequest>();

        for (var i = 0; i < Random.Shared.Next(1, 10); i++)
        {
            var menu = await builder.SeedMenu(restaurant);

            items.Add(new()
            {
                menu_id = menu.Id,
                quantity = (short)Random.Shared.Next(1, 5),
            });
        }

        await builder.Commit();
        var requestBody = new OrderRequest
        {
            items = items,
        };

        await Authenticate(member);

        var response = await _client.PostAsJsonAsync($"orders", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<OrderResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.items.Should().BeEquivalentTo(requestBody.items);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(10)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().Be(null);
    }

    [Fact]
    public async Task Post_Order_Insufficient()
    {
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var branch = await builder.SeedBranch(restaurant);
        var table = await builder.SeedTable(branch);
        var ingredient = await builder.SeedIngredient(restaurant);
        var stockTransaction = await builder.SeedStockTransaction(branch, ingredient, 2);
        var menu = await builder.SeedMenu(restaurant, (ingredient, 10));

        var bill = await builder.SeedBill(table);
        var member = await builder.SeedBillMember(bill);

        await builder.Commit();
        var requestBody = new OrderRequest
        {
            items = [
                new()
                {
                    menu_id = menu.Id,
                    quantity = 3,
                },
            ],
        };

        await Authenticate(member);

        var response = await _client.PostAsJsonAsync($"orders", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict, content);
    }

    [Fact]
    public async Task Get_Order_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(masterUser);
        var branch = await builder.SeedBranch(restaurant);
        var table = await builder.SeedTable(branch);
        var bill = await builder.SeedBill(table);
        var member = await builder.SeedBillMember(bill);
        var order = await builder.SeedOrder(bill);

        await builder.Commit();
        await Authenticate(member);

        var response = await _client.GetAsync($"orders/{order.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<OrderResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(order.Id).And.Be(1);
        responseBody.items.Should().BeEquivalentTo(order.Items);
        responseBody.status.Should().Be(order.Status);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(10)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().Be(null);
    }

    [Fact]
    public async Task Patch_Orders_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var branch = await builder.SeedBranch(restaurant);
        var table = await builder.SeedTable(branch);
        var bill = await builder.SeedBill(table);
        var member = await builder.SeedBillMember(bill);
        Menu[] menus = [.. await builder.SeedMenus(restaurant, 1)];

        var orderRequests = Enumerable.Range(0, Random.Shared.Next(10))
            .Select(_ => new OrderRequest()
            {
                items = Random.Shared.GetItems(menus, Random.Shared.Next(5))
                    .Select(menu => new OrderItemRequest()
                    {
                        menu_id = menu.Id,
                        quantity = (short)Random.Shared.Next(1, 5),
                    })
                    .ToArray(),
            })
            .ToArray();

        await builder.Commit();
        var requestBody = new JsonPatchDocument<IList<OrderRequest>>();

        foreach (var req in orderRequests)
        {
            requestBody.Add(e => e, req);
        }

        await Authenticate(member);

        var response = await _client.PatchAsJsonAsync($"orders", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        // output.WriteLine(JsonSerializer.Serialize(requestBody.Operations));

        var responseBody = await response.Content.ReadFromJsonAsync<ICollection<OrderResponse>>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.Count.Should().Be(orderRequests.Length);

        foreach (var (orderReq, orderRes, i) in orderRequests.Zip(responseBody, Enumerable.Range(1, orderRequests.Length)))
        {
            orderRes.id.Should().Be((short)i);

            foreach (var (ItemReq, itemRes, n) in orderReq.items.Zip(orderRes.items, Enumerable.Range(1, orderReq.items.Count)))
            {
                itemRes.id.Should().Be((short)n);
                itemRes.menu_id.Should().Be(ItemReq.menu_id);
                itemRes.quantity.Should().Be(ItemReq.quantity);
                itemRes.note.Should().Be(ItemReq.note);

                itemRes.create_time.Should().BeLessThan(TimeSpan.FromSeconds(10)).Before(DateTime.UtcNow);
                itemRes.update_time.Should().Be(null);
            }

            orderRes.create_time.Should().BeLessThan(TimeSpan.FromSeconds(10)).Before(DateTime.UtcNow);
            orderRes.update_time.Should().Be(null);
        }
    }
}