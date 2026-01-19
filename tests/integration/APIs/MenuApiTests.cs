using System.Net;
using System.Net.Http.Json;

namespace FoodSphere.Tests.Integration;

public class MenuApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Menu_Should_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateSeedingBuilder();

        var masterUser = await builder.SeedMasterUserAsync();
        var restaurant = await builder.SeedRestaurantAsync(masterUser.Id);

        List<Controllers.Client.MenuIngredientDTO> ingredients = [];

        for (var i = 0; i < Random.Shared.Next(1, 10); i++)
        {
            var ingredient = await builder.SeedIngredientAsync(restaurant.Id);

            ingredients.Add(new()
            {
                ingredient_id = ingredient.Id,
                amount = TestSeedingGenerator.GetAmount(),
            });
        }

        await builder.CommitAsync();
        var requestBody = new Controllers.Client.MenuRequest
        {
            name = $"TEST:menu-name:{unique}",
            display_name = $"TEST:menu-display_name:{unique}",
            description = $"TEST:menu-description:{unique}",
            image_url = $"http://foodsphere.com/img/{unique}.png",
            price = Random.Shared.Next(300),
            ingredients = ingredients,
        };

        var response = await _client.PostAsJsonAsync($"client/restaurants/{restaurant.Id}/menus", requestBody, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseBody = await response.Content.ReadFromJsonAsync<Controllers.Client.MenuResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.ingredients.Should().BeEquivalentTo(ingredients);

        responseBody.name.Should().Be(requestBody.name);
        responseBody.display_name.Should().Be(requestBody.display_name);
        responseBody.description.Should().Be(requestBody.description);
        responseBody.image_url.Should().Be(requestBody.image_url);
        responseBody.price.Should().Be(requestBody.price);
        responseBody.status.Should().Be(Data.MenuStatus.Active);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task Get_Menu_Should_Succeed()
    {
        using var builder = CreateSeedingBuilder();

        var masterUser = await builder.SeedMasterUserAsync();
        var restaurant = await builder.SeedRestaurantAsync(masterUser.Id);
        var menu = await builder.SeedMenuAsync(restaurant.Id);

        await builder.CommitAsync();

        var response = await _client.GetAsync($"client/restaurants/{restaurant.Id}/menus/{menu.Id}", TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadFromJsonAsync<Controllers.Client.MenuResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(menu.Id);
        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.ingredients.Should().BeEquivalentTo(
            menu.MenuIngredients.Select(Controllers.Client.MenuIngredientDTO.FromModel)
        );

        responseBody.name.Should().Be(menu.Name);
        responseBody.display_name.Should().Be(menu.DisplayName);
        responseBody.description.Should().Be(menu.Description);
        responseBody.image_url.Should().Be(menu.ImageUrl);
        responseBody.price.Should().Be(menu.Price);
        responseBody.status.Should().Be(menu.Status);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }
}