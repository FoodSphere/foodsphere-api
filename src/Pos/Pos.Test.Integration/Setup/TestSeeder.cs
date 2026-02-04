using Microsoft.AspNetCore.Identity;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Pos.Test;

public static class TestSeedingGenerator
{
    static readonly string[] units = ["kg", "g", "lb", "oz", "l", "ml", "cup", "tbsp", "tsp", "piece"];

    public static string GetUniqueString() => Guid.NewGuid().ToString();

    public static string GetUnit() => units[Random.Shared.Next(units.Length)];

    public static string GetDomain() => "foodsphere.com";

    public static decimal GetAmount() => Math.Round((decimal)(Random.Shared.NextDouble() * 10), 2);

    public static string GetPhone() => string.Create(10, 0, (span, _) =>
    {
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = (char)('0' + Random.Shared.Next(10));
        }
    });
}

public class TestSeeder(IServiceScope scope, bool disposeScope = false, CancellationToken ct = default) : IDisposable
{
    public async Task<int> CommitAsync()
    {
        var context = scope.ServiceProvider.GetRequiredService<FoodSphereDbContext>();

        return await context.SaveChangesAsync(ct);
    }

    public void Dispose()
    {
        if (disposeScope)
        {
            scope.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public async Task<(MasterUser, string)> SeedMasterUserAsync()
    {
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

        return (user, password);
    }

    public async Task<Restaurant> SeedRestaurantAsync(string ownerId)
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        var restaurantService = scope.ServiceProvider.GetRequiredService<RestaurantService>();

        var restaurant = await restaurantService.CreateRestaurant(
            ownerId,
            name: $"GENERATED.restaurant-name.{unique}",
            displayName: $"GENERATED.restaurant-display_name.{unique}",
            ct: ct);

        return restaurant;
    }

    public async Task<Menu> SeedMenuAsync(Guid restaurantId)
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        var menuService = scope.ServiceProvider.GetRequiredService<MenuService>();

        var menu = await menuService.CreateMenu(
            restaurantId,
            name: $"GENERATED.menu-name.{unique}",
            displayName: $"GENERATED.menu-display_name.{unique}",
            description: $"GENERATED.menu-description.{unique}",
            imageUrl: $"http://foodsphere.com/img/GENERATED.{unique}.png",
            price: Random.Shared.Next(300),
            ct: ct);

        return menu;
    }

    public async Task<Ingredient> SeedIngredientAsync(Guid restaurantId)
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        var ingredientService = scope.ServiceProvider.GetRequiredService<MenuService>();

        var ingredient = await ingredientService.CreateIngredient(
            restaurantId,
            name: $"GENERATED.ingredient-name.{unique}",
            description: $"GENERATED.ingredient-description.{unique}",
            imageUrl: $"http://foodsphere.com/img/GENERATED.{unique}.png",
            unit: TestSeedingGenerator.GetUnit(),
            ct: ct);

        return ingredient;
    }

    public async Task<Branch> SeedBranchAsync(Guid restaurantId)
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        var branchService = scope.ServiceProvider.GetRequiredService<BranchService>();

        var branch = await branchService.CreateBranch(
            restaurantId,
            name: $"GENERATED.branch-name.{unique}",
            displayName: $"GENERATED.branch-display_name.{unique}",
            address: $"GENERATED.branch-address.{unique}",
            openingTime: new TimeOnly(Random.Shared.Next(0, 12), Random.Shared.Next(0, 2) * 30),
            closingTime: new TimeOnly(Random.Shared.Next(12, 24), Random.Shared.Next(0, 2) * 30),
            ct: ct);

        return branch;
    }

    public async Task<Table> SeedTableAsync(Guid restaurantId, short branchId)
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        var tableService = scope.ServiceProvider.GetRequiredService<BranchService>();

        var table = await tableService.CreateTable(
            restaurantId, branchId,
            name: $"GENERATED.table-name.{unique}",
            ct: ct);

        return table;
    }

    public async Task<Permission[]> SeedPermissionAsync()
    {
        var permissionsIds = PERMISSION.GetAll()
            .ToArray();

        Random.Shared.Shuffle(permissionsIds);

        return permissionsIds[..Random.Shared.Next(permissionsIds.Length)];
    }

    public async Task<Role> SeedRoleAsync(Guid restaurantId, params Permission[] permissions)
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        var roleService = scope.ServiceProvider.GetRequiredService<RoleService>();

        var role = await roleService.CreateRole(
            restaurantId,
            name: $"GENERATED.role-name.{unique}",
            description: $"GENERATED.role-description.{unique}",
            ct: ct);

        await roleService.SetPermissionsAsync(
            restaurantId, role.Id,
            permissions.Select(p => p.Id),
            ct: ct);

        return role;
    }
}