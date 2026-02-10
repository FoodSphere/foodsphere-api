using Microsoft.AspNetCore.Identity;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Pos.Test;

public static class TestSeedingGenerator
{
    static readonly string[] units = ["kg", "g", "lb", "oz", "l", "ml", "cup", "tbsp", "tsp", "piece"];

    public static string GetUniqueString() => DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");

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
    public async Task<int> Commit()
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

    public async Task<(MasterUser, string)> SeedMasterUser()
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

    public async Task<Restaurant> SeedRestaurant(string ownerId)
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

    public async Task<Restaurant> SeedRestaurant(MasterUser owner)
    {
        return await SeedRestaurant(owner.Id);
    }

    public async Task<Menu> SeedMenu(Guid restaurantId)
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

    public async Task<Menu> SeedMenu(Restaurant restaurant)
    {
        return await SeedMenu(restaurant.Id);
    }

    public async Task<Ingredient> SeedIngredient(Guid restaurantId)
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        var ingredientService = scope.ServiceProvider.GetRequiredService<IngredientService>();

        var ingredient = await ingredientService.CreateIngredient(
            restaurantId,
            name: $"GENERATED.ingredient-name.{unique}",
            description: $"GENERATED.ingredient-description.{unique}",
            imageUrl: $"http://foodsphere.com/img/GENERATED.{unique}.png",
            unit: TestSeedingGenerator.GetUnit(),
            ct: ct);

        return ingredient;
    }

    public async Task<Ingredient> SeedIngredient(Restaurant restaurant)
    {
        return await SeedIngredient(restaurant.Id);
    }

    public async Task<Branch> SeedBranch(Guid restaurantId)
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

    public async Task<Branch> SeedBranch(Restaurant restaurant)
    {
        return await SeedBranch(restaurant.Id);
    }

    public async Task<Table> SeedTable(Guid restaurantId, short branchId)
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        var tableService = scope.ServiceProvider.GetRequiredService<BranchService>();

        var table = await tableService.CreateTable(
            restaurantId, branchId,
            name: $"GENERATED.table-name.{unique}",
            ct: ct);

        return table;
    }

    public async Task<Table> SeedTable(Branch branch)
    {
        return await SeedTable(branch.RestaurantId, branch.Id);
    }

    public async Task<Permission[]> SeedPermissions()
    {
        var permissionsIds = PERMISSION.GetAll()
            .ToArray();

        Random.Shared.Shuffle(permissionsIds);

        return permissionsIds[..Random.Shared.Next(permissionsIds.Length)];
    }

    public async Task<Role> SeedRole(Guid restaurantId, params IEnumerable<Permission> permissions)
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        var roleService = scope.ServiceProvider.GetRequiredService<RoleService>();

        var role = await roleService.CreateRole(
            restaurantId,
            name: $"GENERATED.role-name.{unique}",
            description: $"GENERATED.role-description.{unique}",
            ct: ct);

        await roleService.SetPermissions(
            restaurantId, role.Id,
            permissions.Select(p => p.Id),
            ct: ct);

        return role;
    }

    public async Task<Role> SeedRole(Restaurant restaurant, params IEnumerable<Permission> permissions)
    {
        return await SeedRole(restaurant.Id, permissions);
    }

    public async Task<Role[]> SeedRoles(Restaurant restaurant)
    {
        var permissions = await SeedPermissions();
        var roles = new List<Role>();

        for (var i = 0; i < Random.Shared.Next(1, 3); i++)
        {
            var role = await SeedRole(restaurant, permissions);

            roles.Add(role);
        }

        return roles.ToArray();
    }

    public async Task<RestaurantManager> SeedRestaurantManager(
        Guid restaurantId,
        string masterUserId
    ) {
        var restaurantService = scope.ServiceProvider.GetRequiredService<RestaurantService>();

        var manager = await restaurantService.CreateManager(
            restaurantId, masterUserId,
            ct: ct);

        return manager;
    }

    public async Task<RestaurantManager> SeedRestaurantManager(
        Restaurant restaurant,
        MasterUser masterUser
    ) {
        return await SeedRestaurantManager(restaurant.Id, masterUser.Id);
    }

    public async Task<RestaurantManager> SeedRestaurantManager(
        Guid restaurantId,
        string masterUserId,
        params IEnumerable<Role> roles
    ) {
        var restaurantService = scope.ServiceProvider.GetRequiredService<RestaurantService>();

        var manager = await SeedRestaurantManager(restaurantId, masterUserId);

        await restaurantService.SetManagerRoles(
            manager,
            roles.Select(r => r.Id),
            ct: ct);

        return manager;
    }

    public async Task<RestaurantManager> SeedRestaurantManager(
        Restaurant restaurant,
        MasterUser masterUser,
        params IEnumerable<Role> roles
    ) {
        return await SeedRestaurantManager(restaurant.Id, masterUser.Id, roles);
    }

    public async Task<RestaurantManager> SeedRestaurantManager(
        Guid restaurantId,
        string masterUserId,
        params IEnumerable<Permission> permissions
    ) {
        var role = await SeedRole(restaurantId, permissions);
        var manager = await SeedRestaurantManager(restaurantId, masterUserId, role);

        return manager;
    }

    public async Task<RestaurantManager> SeedRestaurantManager(
        Restaurant restaurant,
        MasterUser masterUser,
        params IEnumerable<Permission> permissions
    ) {
        return await SeedRestaurantManager(restaurant.Id, masterUser.Id, permissions);
    }

    public async Task<StaffUser> SeedStaff(
        Guid restaurantId,
        short branchId
    ) {
        var unique = TestSeedingGenerator.GetUniqueString();
        var staffService = scope.ServiceProvider.GetRequiredService<StaffService>();

        var staff = await staffService.CreateStaff(
            restaurantId, branchId,
            name: $"GENERATED.role-name.{unique}",
            phone: TestSeedingGenerator.GetPhone(),
            ct: ct);

        return staff;
    }

    public async Task<StaffUser> SeedStaff(Branch branch)
    {
        return await SeedStaff(branch.RestaurantId, branch.Id);
    }

    public async Task<StaffUser> SeedStaff(
        Guid restaurantId,
        short branchId,
        params IEnumerable<Role> roles
    ) {
        var staffService = scope.ServiceProvider.GetRequiredService<StaffService>();

        var staff = await SeedStaff(restaurantId, branchId);

        await staffService.SetRoles(
            staff,
            roles.Select(r => r.Id),
            ct: ct);

        return staff;
    }

    public async Task<StaffUser> SeedStaff(
        Branch branch,
        params IEnumerable<Role> roles
    ) {
        return await SeedStaff(branch.RestaurantId, branch.Id, roles);
    }

    public async Task<StaffUser> SeedStaff(
        Guid restaurantId,
        short branchId,
        params IEnumerable<Permission> permissions
    ) {
        var role = await SeedRole(restaurantId, permissions);
        var staff = await SeedStaff(restaurantId, branchId, role);

        return staff;
    }

    public async Task<StaffUser> SeedStaff(
        Branch branch,
        params IEnumerable<Permission> permissions
    ) {
        return await SeedStaff(branch.RestaurantId, branch.Id, permissions);
    }

    public async Task<Bill> SeedBill(
        Guid restaurantId,
        short branchId,
        short tableId,
        short? pax = null,
        Guid? consumerId = null
    ) {
        var billService = scope.ServiceProvider.GetRequiredService<BillService>();

        var bill = await billService.CreateBill(
            restaurantId, branchId, tableId,
            pax, consumerId,
            ct: ct);

        return bill;
    }

    public async Task<Bill> SeedBill(
        Table table,
        short? pax = null,
        ConsumerUser? consumer = null
    ) {
        return await SeedBill(
            table.RestaurantId, table.BranchId, table.Id,
            pax, consumer?.Id
        );
    }

    public async Task<Order> SeedOrder(
        Bill bill,
        params IEnumerable<(Menu menu, short quantity)> items
    ) {
        var unique = TestSeedingGenerator.GetUniqueString();
        var billService = scope.ServiceProvider.GetRequiredService<BillService>();
        var order = await billService.CreateOrder(bill);

        foreach (var (menu, quantity) in items)
        {
            await billService.CreateOrderItem(
                order, menu,
                quantity,
                note: $"GENERATED.order-item-note.{unique}",
                ct);
        }

        return order;
    }

    public async Task<Tag> SeedTag(Restaurant restaurant) {
        var unique = TestSeedingGenerator.GetUniqueString();
        var tagService = scope.ServiceProvider.GetRequiredService<TagService>();

        var tag = await tagService.CreateTag(
            restaurant,
            name: $"GENERATED.tag-name.{unique}",
            ct: ct);

        return tag;
    }
}