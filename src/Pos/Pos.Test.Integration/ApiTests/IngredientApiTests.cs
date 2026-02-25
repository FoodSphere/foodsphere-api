namespace FoodSphere.Pos.Test.Integration;

public class IngredientApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_Ingredient_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);

        await builder.Commit();
        var requestBody = new IngredientRequest
        {
            name = $"TEST.ingredient-name.{unique}",
            description = $"TEST.ingredient-description.{unique}",
            unit = TestSeedingGenerator.GetUnit()
        };

        await Authenticate(owner);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/ingredients", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<IngredientResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);

        responseBody.name.Should().Be(requestBody.name);
        responseBody.description.Should().Be(requestBody.description);
        responseBody.unit.Should().Be(requestBody.unit);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task Get_Ingredient_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var ingredient = await builder.SeedIngredient(restaurant);

        await builder.Commit();
        await Authenticate(owner);

        var response = await _client.GetAsync($"restaurants/{restaurant.Id}/ingredients/{ingredient.Id}", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<IngredientResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(ingredient.Id).And.Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);

        responseBody.name.Should().Be(ingredient.Name);
        responseBody.description.Should().Be(ingredient.Description);
        responseBody.image_url.Should().Be(ingredient.ImageUrl);
        responseBody.unit.Should().Be(ingredient.Unit);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }
}