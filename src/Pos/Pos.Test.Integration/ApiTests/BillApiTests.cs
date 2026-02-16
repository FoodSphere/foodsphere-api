namespace FoodSphere.Pos.Test.Integration;

public class BillApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Bill_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var branch = await builder.SeedBranch(restaurant);
        var table = await builder.SeedTable(branch);

        await builder.Commit();
        var requestBody = new BillRequest
        {
            table_id = table.Id,
            // consumer_id = null,
            pax = (short)Random.Shared.Next(1, 20),
        };

        await Authenticate(owner);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/branches/{branch.Id}/bills", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<BillResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.branch_id.Should().Be(branch.Id);
        responseBody.table_id.Should().Be(requestBody.table_id);
        responseBody.consumer_id.Should().Be(requestBody.consumer_id);

        responseBody.pax.Should().Be(requestBody.pax);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task Get_Bill_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var branch = await builder.SeedBranch(restaurant);
        var table = await builder.SeedTable(branch);
        var bill = await builder.SeedBill(table);

        await builder.Commit();
        await Authenticate(owner);

        var response = await _client.GetAsync($"restaurants/{restaurant.Id}/branches/{branch.Id}/bills/{bill.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<BillResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(bill.Id);
        responseBody.restaurant_id.Should().Be(bill.RestaurantId);
        responseBody.branch_id.Should().Be(bill.BranchId);
        responseBody.table_id.Should().Be(bill.TableId);
        responseBody.consumer_id.Should().Be(bill.ConsumerId);

        responseBody.orders.Should().BeEquivalentTo(bill.Orders);
        responseBody.pax.Should().Be(bill.Pax);
        responseBody.status.Should().Be(bill.Status);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }
}