namespace FoodSphere.Pos.Test.Integration;

public class MenuApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Owner_Post_Menu_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(masterUser);

        List<IngredientItemDto> ingredients = [];

        for (var i = 0; i < Random.Shared.Next(1, 10); i++)
        {
            var ingredient = await builder.SeedIngredient(restaurant.Id);

            ingredients.Add(new()
            {
                ingredient_id = ingredient.Id,
                amount = TestSeedingGenerator.GetAmount(),
            });
        }

        await builder.Commit();
        var requestBody = new MenuRequest
        {
            name = $"TEST.menu-name.{unique}",
            display_name = $"TEST.menu-display_name.{unique}",
            description = $"TEST.menu-description.{unique}",
            // image_url = $"http://foodsphere.com/img/{unique}.png",
            price = Random.Shared.Next(300),
            ingredients = ingredients,
        };

        await Authenticate(masterUser);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/menus", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<MenuResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.ingredients.Should().BeEquivalentTo(ingredients);

        responseBody.name.Should().Be(requestBody.name);
        responseBody.display_name.Should().Be(requestBody.display_name);
        responseBody.description.Should().Be(requestBody.description);
        responseBody.price.Should().Be(requestBody.price);
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
        await builder.SeedRestaurantManager(restaurant, manager, PERMISSION.Menu.CREATE);

        List<IngredientItemDto> ingredients = [];

        for (var i = 0; i < Random.Shared.Next(1, 10); i++)
        {
            var ingredient = await builder.SeedIngredient(restaurant.Id);

            ingredients.Add(new()
            {
                ingredient_id = ingredient.Id,
                amount = TestSeedingGenerator.GetAmount(),
            });
        }

        await builder.Commit();
        var requestBody = new MenuRequest
        {
            name = $"TEST.menu-name.{unique}",
            display_name = $"TEST.menu-display_name.{unique}",
            description = $"TEST.menu-description.{unique}",
            // image_url = $"http://foodsphere.com/img/{unique}.png",
            price = Random.Shared.Next(300),
            ingredients = ingredients,
        };

        await Authenticate(manager);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/menus", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<MenuResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.id.Should().Be(1);
        responseBody.restaurant_id.Should().Be(restaurant.Id);
        responseBody.ingredients.Should().BeEquivalentTo(ingredients);

        responseBody.name.Should().Be(requestBody.name);
        responseBody.display_name.Should().Be(requestBody.display_name);
        responseBody.description.Should().Be(requestBody.description);
        responseBody.price.Should().Be(requestBody.price);
        responseBody.status.Should().Be(MenuStatus.Active);

        responseBody.create_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        responseBody.update_time.Should().BeLessThan(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
    }

    [Fact]
    public async Task Staff_Post_Menu_Forbidden()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var (manager, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        await builder.SeedRestaurantManager(restaurant, manager);

        List<IngredientItemDto> ingredients = [];

        for (var i = 0; i < Random.Shared.Next(1, 10); i++)
        {
            var ingredient = await builder.SeedIngredient(restaurant.Id);

            ingredients.Add(new()
            {
                ingredient_id = ingredient.Id,
                amount = TestSeedingGenerator.GetAmount(),
            });
        }

        await builder.Commit();
        var requestBody = new MenuRequest
        {
            name = $"TEST.menu-name.{unique}",
            display_name = $"TEST.menu-display_name.{unique}",
            description = $"TEST.menu-description.{unique}",
            price = Random.Shared.Next(300),
            ingredients = ingredients,
        };

        await Authenticate(manager);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/menus", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden, content);
    }

    [Fact]
    public async Task Owner_Get_Menu_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(masterUser);
        var menu = await builder.SeedMenu(restaurant);

        await builder.Commit();
        await Authenticate(masterUser);

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

    [Fact]
    public async Task Add_Menu_Tag_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(masterUser);
        var menu = await builder.SeedMenu(restaurant);
        var tag = await builder.SeedTag(restaurant);

        await builder.Commit();
        var requestBody = new AssignTagRequest
        {
            tag_id = tag.Id,
        };

        await Authenticate(masterUser);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/menus/{menu.Id}/tags", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent, content);
    }
}