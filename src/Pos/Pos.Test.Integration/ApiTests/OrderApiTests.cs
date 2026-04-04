namespace FoodSphere.Pos.Test.Integration;

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

        await Authenticate(owner);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/branches/{branch.Id}/bills/{bill.Id}/orders", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<OrderResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.bill_id.Should().Be(bill.Id);

        responseBody.items.Should().BeEquivalentTo(requestBody.items);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(10)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().Be(null);
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
        var order = await builder.SeedOrder(bill);

        await builder.Commit();
        await Authenticate(masterUser);

        var response = await _client.GetAsync($"restaurants/{restaurant.Id}/branches/{branch.Id}/bills/{bill.Id}/orders/{order.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<OrderResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(order.Id).And.Be(1);
        responseBody.bill_id.Should().Be(order.BillId);

        responseBody.items.Should().BeEquivalentTo(order.Items);
        responseBody.status.Should().Be(order.Status);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(10)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().Be(null);
    }

    [Fact]
    public async Task List_Orders_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(masterUser);
        var branch = await builder.SeedBranch(restaurant);
        var table = await builder.SeedTable(branch);
        var bill = await builder.SeedBill(table);
        var menus = await builder.SeedMenus(restaurant, 1);
        var orders = await builder.SeedOrders(bill, menus);

        await builder.Commit();
        await Authenticate(masterUser);

        var response = await _client.GetAsync($"restaurants/{restaurant.Id}/branches/{branch.Id}/bills/{bill.Id}/orders", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<List<OrderResponse>>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();
        responseBody.Count.Should().Be(orders.Count);
        // output.WriteLine(content);

        foreach (var (order, orderRes, i) in orders.Zip(responseBody, Enumerable.Range(1, orders.Count)))
        {
            orderRes.id.Should().Be(order.Id).And.Be((short)i);
            orderRes.items.Count.Should().Be(order.Items.Count);

            foreach (var (item, itemRes, n) in order.Items.Zip(orderRes.items, Enumerable.Range(1, order.Items.Count)))
            {
                itemRes.id.Should().Be(item.Id).And.Be((short)n);

                itemRes.menu_id.Should().Be(item.MenuId);
                itemRes.quantity.Should().Be(item.Quantity);
                itemRes.note.Should().Be(item.Note);

                itemRes.create_time.Should().BeLessThan(TimeSpan.FromSeconds(10)).Before(DateTime.UtcNow);
                itemRes.update_time.Should().Be(null);
            }

            orderRes.create_time.Should().BeLessThan(TimeSpan.FromSeconds(10)).Before(DateTime.UtcNow);
            orderRes.update_time.Should().Be(null);
        }
    }

    [Fact]
    public async Task Update_Order_To_Cooking_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(masterUser);
        var branch = await builder.SeedBranch(restaurant);
        var table = await builder.SeedTable(branch);
        var bill = await builder.SeedBill(table);
        var menus = await builder.SeedMenus(restaurant, 1);
        var order = await builder.SeedOrder(bill, menus);

        await builder.Commit();
        var requestBody = new OrderStatusRequest
        {
            status = OrderStatus.Cooking,
        };

        await Authenticate(masterUser);

        var response = await _client.PutAsJsonAsync($"s/restaurants/{restaurant.Id}/bills/{bill.Id}/orders/{order.Id}/status", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent, content);
    }
}