namespace FoodSphere.Pos.Test.Integration;

public class StaffApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Staff_Should_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUserAsync();
        var restaurant = await builder.SeedRestaurantAsync(masterUser.Id);
        var branch = await builder.SeedBranchAsync(restaurant.Id);
        var permissions = await builder.SeedPermissionAsync();

        List<Role> roles = [];

        for (var i = 0; i < Random.Shared.Next(1, 3); i++)
        {
            var role = await builder.SeedRoleAsync(restaurant.Id, permissions);

            roles.Add(role);
        }

        await builder.CommitAsync();
        var requestBody = new StaffRequest
        {
            name = $"TEST.staff-name.{unique}",
            roles = [.. roles.Select(r => r.Id)],
            phone = TestSeedingGenerator.GetPhone(),
        };

        await Authenticate(masterUser);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/branches/{branch.Id}/staffs", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<StaffResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.branch_id.Should().Be(branch.Id);

        responseBody.name.Should().Be(requestBody.name);
        responseBody.phone.Should().Be(requestBody.phone);
        responseBody.roles.Should().BeEquivalentTo(
            roles.Select(r => r.Id));

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task Get_Staff_Should_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUserAsync();
        var restaurant = await builder.SeedRestaurantAsync(masterUser.Id);
        var branch = await builder.SeedBranchAsync(restaurant.Id);
        var permissions = await builder.SeedPermissionAsync();

        List<Role> roles = [];

        for (var i = 0; i < Random.Shared.Next(1, 3); i++)
        {
            var role = await builder.SeedRoleAsync(restaurant.Id, permissions);

            roles.Add(role);
        }

        var staff = await builder.SeedStaffAsync(
            restaurant.Id,
            branch.Id,
            roles: roles
        );

        await builder.CommitAsync();
        await Authenticate(masterUser);

        var response = await _client.GetAsync($"restaurants/{restaurant.Id}/branches/{branch.Id}/staffs/{staff.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<StaffResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(staff.Id);
        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.branch_id.Should().Be(branch.Id);

        responseBody.name.Should().Be(staff.Name);
        responseBody.phone.Should().Be(staff.Phone);
        responseBody.roles.Should().BeEquivalentTo(
            roles.Select(r => r.Id));

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }
}