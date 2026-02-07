namespace FoodSphere.Pos.Test.Integration;

public class BranchApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Branch_Should_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(masterUser);

        await builder.Commit();
        var requestBody = new BranchRequest
        {
            name = $"TEST.branch-name.{unique}",
            display_name = $"TEST.branch-display_name.{unique}",
            address = $"TEST.branch-address.{unique}",
            opening_time = new TimeOnly(Random.Shared.Next(0, 12), Random.Shared.Next(0, 2) * 30),
            closing_time = new TimeOnly(Random.Shared.Next(12, 24), Random.Shared.Next(0, 2) * 30),
            contact = new()
            {
                name = $"TEST.branch-contact-name.{unique}",
                email = $"{unique}@{TestSeedingGenerator.GetDomain()}",
                phone = TestSeedingGenerator.GetPhone(),
            },
        };

        await Authenticate(masterUser);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/branches", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<BranchResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.contact.Should().BeEquivalentTo(requestBody.contact);

        responseBody.name.Should().Be(requestBody.name);
        responseBody.display_name.Should().Be(requestBody.display_name);
        responseBody.address.Should().Be(requestBody.address);
        responseBody.opening_time.Should().Be(requestBody.opening_time);
        responseBody.closing_time.Should().Be(requestBody.closing_time);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task Get_Branch_Should_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(masterUser);
        var branch = await builder.SeedBranch(restaurant);

        await builder.Commit();
        await Authenticate(masterUser);

        var response = await _client.GetAsync($"restaurants/{restaurant.Id}/branches/{branch.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<BranchResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(branch.Id).And.Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.contact.Should().BeEquivalentTo(branch.Contact);

        responseBody.name.Should().Be(branch.Name);
        responseBody.display_name.Should().Be(branch.DisplayName);
        responseBody.address.Should().Be(branch.Address);
        responseBody.opening_time.Should().Be(branch.OpeningTime);
        responseBody.closing_time.Should().Be(branch.ClosingTime);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }
}