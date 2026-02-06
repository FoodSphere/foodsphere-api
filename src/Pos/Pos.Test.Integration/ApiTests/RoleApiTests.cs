namespace FoodSphere.Pos.Test.Integration;

public class RoleApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Role_Should_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUserAsync();
        var restaurant = await builder.SeedRestaurantAsync(masterUser);
        var permissions = await builder.SeedPermissionAsync();

        await builder.CommitAsync();
        var requestBody = new RoleRequest
        {
            name = $"TEST.role-name.{unique}",
            description = $"TEST.role-description.{unique}",
            permission_ids = [.. permissions.Select(p => p.Id)],
        };

        await Authenticate(masterUser);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/roles", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<RoleResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);

        responseBody.name.Should().Be(requestBody.name);
        responseBody.description.Should().Be(requestBody.description);
        responseBody.permissions.Should().BeEquivalentTo(
            permissions.Select(PermissionResponse.FromModel));

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task Get_Role_Should_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUserAsync();
        var restaurant = await builder.SeedRestaurantAsync(masterUser);
        var permissions = await builder.SeedPermissionAsync();
        var role = await builder.SeedRoleAsync(restaurant, permissions);

        await builder.CommitAsync();
        await Authenticate(masterUser);

        var response = await _client.GetAsync($"restaurants/{restaurant.Id}/roles/{role.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<RoleResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(role.Id).And.Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);

        responseBody.name.Should().Be(role.Name);
        responseBody.description.Should().Be(role.Description);
        responseBody.permissions.Should().BeEquivalentTo(
            permissions.Select(PermissionResponse.FromModel));

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }
}