using System.Net;
using System.Net.Http.Json;

namespace FoodSphere.Tests.Integration;

public class IngredientApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Ingredient_Should_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateSeedingBuilder();

        var (masterUser, _) = await builder.SeedMasterUserAsync();
        var restaurant = await builder.SeedRestaurantAsync(masterUser.Id);

        await builder.CommitAsync();
        var requestBody = new Controllers.Client.IngredientRequest
        {
            name = $"TEST.ingredient-name.{unique}",
            description = $"TEST.ingredient-description.{unique}",
            image_url = $"http://foodsphere.com/img/{unique}.png",
            unit = TestSeedingGenerator.GetUnit()
        };

        await Authenticate(masterUser);

        var response = await _client.PostAsJsonAsync($"client/restaurants/{restaurant.Id}/ingredients", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<Controllers.Client.IngredientResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);

        responseBody.name.Should().Be(requestBody.name);
        responseBody.description.Should().Be(requestBody.description);
        responseBody.image_url.Should().Be(requestBody.image_url);
        responseBody.unit.Should().Be(requestBody.unit);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task Get_Ingredient_Should_Succeed()
    {
        using var builder = CreateSeedingBuilder();

        var (masterUser, _) = await builder.SeedMasterUserAsync();
        var restaurant = await builder.SeedRestaurantAsync(masterUser.Id);
        var ingredient = await builder.SeedIngredientAsync(restaurant.Id);

        await builder.CommitAsync();
        await Authenticate(masterUser);

        var response = await _client.GetAsync($"client/restaurants/{restaurant.Id}/ingredients/{ingredient.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<Controllers.Client.IngredientResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(ingredient.Id);
        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);

        responseBody.name.Should().Be(ingredient.Name);
        responseBody.description.Should().Be(ingredient.Description);
        responseBody.image_url.Should().Be(ingredient.ImageUrl);
        responseBody.unit.Should().Be(ingredient.Unit);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }
}