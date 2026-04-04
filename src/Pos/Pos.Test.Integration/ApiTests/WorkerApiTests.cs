namespace FoodSphere.Pos.Test.Integration;

public class WorkerApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Worker_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var branch = await builder.SeedBranch(restaurant);
        var roles = await builder.SeedRoles(restaurant);

        await builder.Commit();
        var requestBody = new WorkerRequest
        {
            name = $"TEST.worker-name.{unique}",
            roles = [.. roles.Select(r => r.Id)],
            phone = TestSeedingGenerator.GetPhone(),
        };

        await Authenticate(owner);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/branches/{branch.Id}/workers", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<WorkerResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.branch_id.Should().Be(branch.Id);

        responseBody.name.Should().Be(requestBody.name);
        responseBody.phone.Should().Be(requestBody.phone);
        responseBody.roles.Should().BeEquivalentTo(
            roles.Select(r => r.Id));

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(10)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().Be(null);
    }

    [Fact]
    public async Task Get_Worker_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var branch = await builder.SeedBranch(restaurant);
        var roles = await builder.SeedRoles(restaurant);
        var worker = await builder.SeedWorker(
            branch, roles.Select(r => new RoleKey(restaurant.Id, r.Id)));

        await builder.Commit();
        await Authenticate(owner);

        var response = await _client.GetAsync($"restaurants/{restaurant.Id}/branches/{branch.Id}/workers/{worker.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<WorkerResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(worker.Id).And.Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.branch_id.Should().Be(branch.Id);

        responseBody.name.Should().Be(worker.Name);
        responseBody.phone.Should().Be(worker.Phone);
        responseBody.roles.Should().BeEquivalentTo(
            roles.Select(r => r.Id));

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(10)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().Be(null);
    }
}