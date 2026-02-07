namespace FoodSphere.Pos.Test.Integration;

public class OrderApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Order_Should_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(masterUser);
        var branch = await builder.SeedBranch(restaurant);
        var table = await builder.SeedTable(branch);
        var bill = await builder.SeedBill(table);

        List<OrderItemDto> items = [];

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
        var requestBody = new OrderDto
        {
            items = items,
        };

        await Authenticate(masterUser);

        var response = await _client.PostAsJsonAsync($"bills/{bill.Id}/orders", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<OrderResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.bill_id.Should().Be(bill.Id);

        responseBody.items.Should().BeEquivalentTo(requestBody.items);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task Get_Bill_Should_Succeed()
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

        var response = await _client.GetAsync($"bills/{bill.Id}/orders/{order.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<OrderResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(order.Id).And.Be(1);
        responseBody.bill_id.Should().Be(order.BillId);

        responseBody.items.Should().BeEquivalentTo(order.Items);
        responseBody.status.Should().Be(order.Status);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }
}