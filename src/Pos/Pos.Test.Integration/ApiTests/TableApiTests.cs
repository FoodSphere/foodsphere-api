namespace FoodSphere.Pos.Test.Integration;

public class TableApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Table_Should_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUserAsync();
        var restaurant = await builder.SeedRestaurantAsync(masterUser);
        var branch = await builder.SeedBranchAsync(restaurant);

        await builder.CommitAsync();
        var requestBody = new TableRequest
        {
            name = $"TEST.table-name.{unique}",
        };

        await Authenticate(masterUser);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/branches/{branch.Id}/tables", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<TableResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.branch_id.Should().Be(branch.Id);

        responseBody.name.Should().Be(requestBody.name);
        // responseBody.status.Should().Be(TableStatus.Open);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task Get_Table_Should_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUserAsync();
        var restaurant = await builder.SeedRestaurantAsync(masterUser);
        var branch = await builder.SeedBranchAsync(restaurant);
        var table = await builder.SeedTableAsync(branch);

        await builder.CommitAsync();
        await Authenticate(masterUser);

        var response = await _client.GetAsync($"restaurants/{restaurant.Id}/branches/{branch.Id}/tables/{table.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<TableResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(table.Id).And.Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.branch_id.Should().Be(branch.Id);

        responseBody.name.Should().Be(table.Name);
        responseBody.status.Should().Be(table.Status);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }
}