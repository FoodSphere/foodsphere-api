using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using FoodSphere.Data.Models;
using FoodSphere.Services;

namespace FoodSphere.Tests;

public record TestUserData(
    string Id,
    string Email,
    string Password
);

public static class TestSeedingGenerator
{
    static readonly string[] unit = ["kg", "g", "lb", "oz", "l", "ml", "cup", "tbsp", "tsp", "piece"];

    public static string GetUniqueString() => Guid.NewGuid().ToString();
    public static string GetUnit() => unit[Random.Shared.Next(unit.Length)];
    public static string GetDomain() => "foodsphere.com";

    public static decimal GetAmount() => Math.Round((decimal)(Random.Shared.NextDouble() * 10), 2);

    public static string GetPhone()
    {
        var sb = new StringBuilder(15);

        // use Enumerable?
        for (var i = 0; i < sb.MaxCapacity; i++)
        {
            sb.Append(Random.Shared.Next(10));
        }

        return sb.ToString();
    }
}

public class TestSeedingBuilder(IServiceScope scope, bool disposeScope = false) : IDisposable
{
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        if (disposeScope)
        {
            scope.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public async Task<TestUserData> SeedMasterAsync(
        CancellationToken cancellationToken = default
    ) {
        var unique = TestSeedingGenerator.GetUniqueString();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<MasterUser>>();

        var email = $"{unique}@{TestSeedingGenerator.GetDomain()}";
        var password = unique;

        var user = new MasterUser()
        {
            UserName = email,
            Email = email,
        };

        await userManager.CreateAsync(user, password);

        return new(
            user.Id,
            email,
            password
        );
    }

    public async Task<Restaurant> SeedRestaurantAsync(
        string ownerId,
        CancellationToken cancellationToken = default
    ) {
        var unique = TestSeedingGenerator.GetUniqueString();
        var restaurantService = scope.ServiceProvider.GetRequiredService<RestaurantService>();

        var restaurant = await restaurantService.CreateRestaurant(
            ownerId: ownerId,
            name: $"restaurant-name:{unique}",
            displayName: $"restaurant-display_name:{unique}",
            cancellationToken: cancellationToken
        );

        return restaurant;
    }

    public async Task<Menu> SeedMenuAsync(
        Guid restaurantId,
        CancellationToken cancellationToken = default
    ) {
        var unique = TestSeedingGenerator.GetUniqueString();
        var menuService = scope.ServiceProvider.GetRequiredService<MenuService>();

        var menu = await menuService.CreateMenu(
            restaurantId: restaurantId,
            name: $"menu-name:{unique}",
            displayName: $"menu-display_name:{unique}",
            description: $"menu-description:{unique}",
            imageUrl: $"http://foodsphere.com/img/{unique}.png",
            price: Random.Shared.Next(300),
            cancellationToken: cancellationToken
        );

        return menu;
    }

    public async Task<Ingredient> SeedIngredientAsync(
        Guid restaurantId,
        CancellationToken cancellationToken = default
    ) {
        var unique = TestSeedingGenerator.GetUniqueString();
        var ingredientService = scope.ServiceProvider.GetRequiredService<MenuService>();

        var ingredient = await ingredientService.CreateIngredient(
            restaurantId: restaurantId,
            name: $"ingredient-name:{unique}",
            description: $"ingredient-description:{unique}",
            imageUrl: $"http://foodsphere.com/img/{unique}.png",
            unit: TestSeedingGenerator.GetUnit(),
            cancellationToken: cancellationToken
        );

        return ingredient;
    }

    public async Task<Branch> SeedBranchAsync(
        Guid restaurantId,
        CancellationToken cancellationToken = default
    ) {
        var unique = TestSeedingGenerator.GetUniqueString();
        var branchService = scope.ServiceProvider.GetRequiredService<BranchService>();

        var branch = await branchService.CreateBranch(
            restaurantId: restaurantId,
            name: $"branch-name:{unique}",
            displayName: $"branch-display_name:{unique}",
            address: $"branch-address:{unique}",
            openingTime: new TimeOnly(Random.Shared.Next(0, 12), Random.Shared.Next(0, 2) * 30),
            closingTime: new TimeOnly(Random.Shared.Next(12, 24), Random.Shared.Next(0, 2) * 30),
            cancellationToken: cancellationToken
        );

        return branch;
    }

    public async Task<Table> SeedTableAsync(
        Guid restaurantId,
        short branchId,
        CancellationToken cancellationToken = default
    ) {
        var unique = TestSeedingGenerator.GetUniqueString();
        var tableService = scope.ServiceProvider.GetRequiredService<BranchService>();

        var table = await tableService.CreateTable(
            restaurantId: restaurantId,
            branchId: branchId,
            name: $"table-name:{unique}",
            cancellationToken: cancellationToken
        );

        return table;
    }
}