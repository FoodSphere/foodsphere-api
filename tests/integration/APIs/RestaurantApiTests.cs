using System.Net;
using System.Net.Http.Json;

namespace FoodSphere.Tests.Integration;

public class RestaurantApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Restaurant_Should_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateSeedingBuilder();

        var (masterUser, _) = await builder.SeedMasterUserAsync();

        await builder.CommitAsync();
        var requestBody = new Controllers.Client.QuickRestaurantRequest
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

        await Authenticate(masterUser);

        var response = await _client.PostAsJsonAsync("client/restaurants", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<Controllers.Client.QuickRestaurantResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.restaurant_id.Should().NotBeEmpty();
        responseBody.restaurant_contact.Should().BeEquivalentTo(requestBody.contact);
        responseBody.restaurant_name.Should().Be(requestBody.name);
        responseBody.restaurant_display_name.Should().Be(requestBody.display_name);

        responseBody.branch_id.Should().Be(1);
        responseBody.branch_name.Should().Be("main");
        responseBody.branch_address.Should().Be(requestBody.address);
        responseBody.branch_opening_time.Should().Be(requestBody.opening_time);
        responseBody.branch_closing_time.Should().Be(requestBody.closing_time);
    }

    [Fact]
    public async Task Get_Restaurant_Should_Succeed()
    {
        using var builder = CreateSeedingBuilder();

        var (masterUser, _) = await builder.SeedMasterUserAsync();
        var restaurant = await builder.SeedRestaurantAsync(masterUser.Id);

        await builder.CommitAsync();
        await Authenticate(masterUser);

        var response = await _client.GetAsync($"client/restaurants/{restaurant.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<Controllers.Client.RestaurantResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(restaurant.Id);
        responseBody.contact.Should().BeEquivalentTo(
            Data.DTOs.ContactDTO.FromModel(restaurant.Contact)
        );

        responseBody.name.Should().Be(restaurant.Name);
        responseBody.display_name.Should().Be(restaurant.DisplayName);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }
}