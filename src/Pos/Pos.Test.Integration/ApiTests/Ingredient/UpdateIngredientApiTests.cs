namespace FoodSphere.Pos.Test.Integration;

public class UpdateIngredientApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Update_Single_Ingredient_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var branch = await builder.SeedBranch(restaurant);
        var ingredient = await builder.SeedIngredient(restaurant);
        var tags = await builder.SeedTags(restaurant);

        await builder.Commit();
        var requestBody = new IngredientRequest
        {
            name = $"TEST.ingredient-name.{unique}",
            tags = tags.ToAssignTagRequests(),
            unit = TestSeedingGenerator.GetUnit(),
            description = $"TEST.ingredient-description.{unique}",
            status = IngredientStatus.Active,
        };

        await Authenticate(owner);

        var response = await _client.PutAsJsonAsync($"s/restaurants/{restaurant.Id}/ingredients/{ingredient.Id}", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent, content);
    }
}