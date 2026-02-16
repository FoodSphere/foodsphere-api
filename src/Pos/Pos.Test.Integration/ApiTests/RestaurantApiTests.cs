namespace FoodSphere.Pos.Test.Integration;

public class RestaurantApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Restaurant_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();

        await builder.Commit();
        var requestBody = new SingleRestaurantRequest
        {
            name = $"TEST.restaurant-name.{unique}",
            display_name = $"TEST.restaurant-display_name.{unique}",
            address = $"TEST.branch-address.{unique}",
            opening_time = new TimeOnly(Random.Shared.Next(0, 12), Random.Shared.Next(0, 2) * 30),
            closing_time = new TimeOnly(Random.Shared.Next(12, 24), Random.Shared.Next(0, 2) * 30),
            contact = new()
            {
                name = $"TEST.contact-name.{unique}",
                email = $"{unique}@{TestSeedingGenerator.GetDomain()}",
                phone = TestSeedingGenerator.GetPhone(),
            },
        };

        await Authenticate(owner);

        var response = await _client.PostAsJsonAsync("restaurants", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<RestaurantResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().NotBeEmpty();
        responseBody.contact.Should().BeEquivalentTo(requestBody.contact);
        responseBody.name.Should().Be(requestBody.name);
        responseBody.display_name.Should().Be(requestBody.display_name);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task Get_Restaurant_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner.Id);

        await builder.Commit();
        await Authenticate(owner);

        var response = await _client.GetAsync($"restaurants/{restaurant.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<RestaurantResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(restaurant.Id);
        responseBody.contact.Should().BeEquivalentTo(
            ContactDto.Project(restaurant.Contact));

        responseBody.name.Should().Be(restaurant.Name);
        responseBody.display_name.Should().Be(restaurant.DisplayName);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }
}