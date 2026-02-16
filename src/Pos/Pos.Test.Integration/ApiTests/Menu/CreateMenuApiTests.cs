namespace FoodSphere.Pos.Test.Integration;

public class CreateMenuApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Owner_Post_Menu_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var tags = await builder.SeedTags(restaurant);
        var ingredients = await builder.SeedIngredients(restaurant);
        var ingredientItemsResponses = ingredients.ToIngredientItemResponses();

        await builder.Commit();
        var requestBody = new MenuRequest
        {
            name = $"TEST.menu-name.{unique}",
            display_name = $"TEST.menu-display_name.{unique}",
            description = $"TEST.menu-description.{unique}",
            price = Random.Shared.Next(300),
            ingredients = ingredientItemsResponses.ToIngredientItemDtos(),
            tags = tags.ToAssignTagRequests(),
        };

        await Authenticate(owner);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/menus", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<MenuResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);

        responseBody.ingredients.Should().BeEquivalentTo(ingredientItemsResponses);
        responseBody.tags.Should().BeEquivalentTo(tags.ToTagDtos());

        responseBody.name.Should().Be(requestBody.name);
        responseBody.price.Should().Be(requestBody.price);
        responseBody.display_name.Should().Be(requestBody.display_name);
        responseBody.description.Should().Be(requestBody.description);
        responseBody.image_url.Should().BeNull();
        responseBody.status.Should().Be(MenuStatus.Active);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task RestaurantManager_Post_Menu_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var (manager, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var tags = await builder.SeedTags(restaurant);
        var ingredients = await builder.SeedIngredients(restaurant);
        var ingredientItemsResponses = ingredients.ToIngredientItemResponses();

        await builder.SeedRestaurantManager(restaurant, manager, PERMISSION.Menu.CREATE);

        await builder.Commit();
        var requestBody = new MenuRequest
        {
            name = $"TEST.menu-name.{unique}",
            display_name = $"TEST.menu-display_name.{unique}",
            description = $"TEST.menu-description.{unique}",
            price = Random.Shared.Next(300),
            ingredients = ingredientItemsResponses.ToIngredientItemDtos(),
            tags = tags.ToAssignTagRequests(),
        };

        await Authenticate(manager);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/menus", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<MenuResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);

        responseBody.ingredients.Should().BeEquivalentTo(ingredientItemsResponses);
        responseBody.tags.Should().BeEquivalentTo(tags.ToTagDtos());

        responseBody.name.Should().Be(requestBody.name);
        responseBody.price.Should().Be(requestBody.price);
        responseBody.display_name.Should().Be(requestBody.display_name);
        responseBody.description.Should().Be(requestBody.description);
        responseBody.image_url.Should().BeNull();
        responseBody.status.Should().Be(MenuStatus.Active);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task RestaurantManager_Post_Menu_Forbidden()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var (manager, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);

        await builder.SeedRestaurantManager(restaurant, manager);

        await builder.Commit();
        var requestBody = new MenuRequest
        {
            name = $"TEST.menu-name.{unique}",
            price = Random.Shared.Next(300),
        };

        await Authenticate(manager);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/menus", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden, content);
    }
}