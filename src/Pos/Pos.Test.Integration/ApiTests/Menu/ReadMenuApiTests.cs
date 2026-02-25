namespace FoodSphere.Pos.Test.Integration;

public class ReadMenuApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Owner_Get_Menu_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var menu = await builder.SeedMenu(restaurant);

        await builder.Commit();
        await Authenticate(owner);

        var response = await _client.GetAsync($"restaurants/{restaurant.Id}/menus/{menu.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<MenuResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(menu.Id).And.Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.ingredients.Should().BeEquivalentTo(
            menu.MenuIngredients.Select(IngredientItemDto.FromModel));

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