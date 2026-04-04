using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using FoodSphere.Common.Constant;
using FoodSphere.Common.Entity;
using FoodSphere.Infrastructure.Persistence;
using FoodSphere.Infrastructure.Repository;
using Bogus;

namespace FoodSphere.Common.Test;

public static class TestSeedingGenerator
{
    static readonly string[] units = [
        "kg", "g", "lb", "oz", "l", "ml", "cup", "tbsp", "tsp", "piece"];

    // DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") too close
    public static string GetUniqueString() => Guid.CreateVersion7().ToString();

    public static string GetUnit() => units[Random.Shared.Next(units.Length)];

    public static string GetDomain() => "foodsphere.com";

    public static decimal GetAmount() => Math.Round(
        (decimal)(Random.Shared.NextDouble() * 10), 2);

    public static string GetPhone() => string.Create(10, 0, (span, _) =>
    {
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = (char)('0' + Random.Shared.Next(10));
        }
    });
}

public class TestSeeder(
    IServiceScope scope,
    bool disposeScope = false,
    CancellationToken ct = default) : IDisposable
{
    public async Task<int> Commit()
    {
        var context = scope.ServiceProvider
            .GetRequiredService<FoodSphereDbContext>();

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

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<MasterUser>>();

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

    public async Task<Restaurant> SeedRestaurant(MasterUserKey ownerKey)
    {
        var unique = TestSeedingGenerator.GetUniqueString();

        var restaurantRepository = scope.ServiceProvider
            .GetRequiredService<RestaurantRepository>();

        var result = await restaurantRepository.CreateRestaurant(
            ownerKey,
            name: $"GENERATED.restaurant-name.{unique}",
            displayName: $"GENERATED.restaurant-display_name.{unique}",
            contact: null,
            ct: ct);

        return result.Value;
    }

    public async Task<Menu> SeedMenu(
        RestaurantKey restaurantKey,
        params IEnumerable<(Ingredient, decimal)> ingredients)
    {
        var unique = TestSeedingGenerator.GetUniqueString();

        var menuRepository = scope.ServiceProvider
            .GetRequiredService<MenuRepository>();

        var result = await menuRepository.CreateMenu(
            restaurantKey,
            name: $"GENERATED.menu-name.{unique}",
            displayName: $"GENERATED.menu-display_name.{unique}",
            description: $"GENERATED.menu-description.{unique}",
            imageUrl: $"http://foodsphere.com/img/GENERATED.{unique}.png",
            price: Random.Shared.Next(300),
            ct: ct);

        var menu = result.Value;

        foreach (var (ingredient, amount) in ingredients)
        {
            var ingredientResult = await menuRepository.CreateMenuIngredient(
                menu, ingredient, amount, ct);
        }

        return menu;
    }

    public async Task<ICollection<Menu>> SeedMenus(
        RestaurantKey restaurantKey, int min = 0, int max = 20)
    {
        var menus = new List<Menu>();

        for (var i = 0; i < Random.Shared.Next(min, max); i++)
        {
            var menu = await SeedMenu(restaurantKey);

            menus.Add(menu);
        }

        return menus;
    }

    public async Task<Ingredient> SeedIngredient(RestaurantKey restaurantKey)
    {
        var unique = TestSeedingGenerator.GetUniqueString();

        var ingredientRepository = scope.ServiceProvider
            .GetRequiredService<IngredientRepository>();

        var result = await ingredientRepository.CreateIngredient(
            restaurantKey,
            name: $"GENERATED.ingredient-name.{unique}",
            description: $"GENERATED.ingredient-description.{unique}",
            imageUrl: $"http://foodsphere.com/img/GENERATED.{unique}.png",
            unit: TestSeedingGenerator.GetUnit(),
            ct: ct);

        return result.Value;
    }

    public async Task<ICollection<Ingredient>> SeedIngredients(
        RestaurantKey restaurantKey)
    {
        var ingredients = new List<Ingredient>();

        for (var i = 0; i < Random.Shared.Next(6); i++)
        {
            var ingredient = await SeedIngredient(restaurantKey);

            ingredients.Add(ingredient);
        }

        return ingredients;
    }

    public async Task<Branch> SeedBranch(RestaurantKey restaurantKey)
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        var branchRepository = scope.ServiceProvider
            .GetRequiredService<BranchRepository>();

        var result = await branchRepository.CreateBranch(
            restaurantKey,
            name: $"GENERATED.branch-name.{unique}",
            displayName: $"GENERATED.branch-display_name.{unique}",
            address: $"GENERATED.branch-address.{unique}",
            openingTime: new TimeOnly(
                Random.Shared.Next(0, 12), Random.Shared.Next(0, 2) * 30),

            closingTime: new TimeOnly(
                Random.Shared.Next(12, 24), Random.Shared.Next(0, 2) * 30),

            contact: null,
            ct: ct);

        return result.Value;
    }

    public async Task<Table> SeedTable(BranchKey branchKey)
    {
        var unique = TestSeedingGenerator.GetUniqueString();

        var tableRepository = scope.ServiceProvider
            .GetRequiredService<TableRepository>();

        var result = await tableRepository.CreateTable(
            branchKey,
            name: $"GENERATED.table-name.{unique}",
            ct: ct);

        return result.Value;
    }

    public async Task<ICollection<Permission>> SeedPermissions()
    {
        var permissionsIds = PERMISSION.GetAll().ToArray();

        Random.Shared.Shuffle(permissionsIds);

        return permissionsIds[..Random.Shared.Next(permissionsIds.Length)];
    }

    public async Task<Role> SeedRole(
        RestaurantKey restaurantKey,
        params IEnumerable<PermissionKey> permissionKeys)
    {
        var unique = TestSeedingGenerator.GetUniqueString();

        var roleRepository = scope.ServiceProvider
            .GetRequiredService<RoleRepository>();

        var roleResult = await roleRepository.CreateRole(
            restaurantKey,
            name: $"GENERATED.role-name.{unique}",
            description: $"GENERATED.role-description.{unique}",
            ct: ct);

        var role = roleResult.Value;

        await roleRepository.SetPermissions(
            role, permissionKeys, ct: ct);

        return role;
    }

    public async Task<ICollection<Role>> SeedRoles(
        RestaurantKey restaurantKey)
    {
        var permissions = await SeedPermissions();
        var roles = new List<Role>();

        for (var i = 0; i < Random.Shared.Next(1, 3); i++)
        {
            var role = await SeedRole(
                restaurantKey,
                permissions.Select(e =>
                    new PermissionKey(e.Id)));

            roles.Add(role);
        }

        return roles;
    }

    public async Task<RestaurantStaff> SeedRestaurantStaff(
        RestaurantKey restaurantKey,
        MasterUserKey masterKey)
    {
        var unique = TestSeedingGenerator.GetUniqueString();

        var staffRepository = scope.ServiceProvider
            .GetRequiredService<RestaurantStaffRepository>();

        var result = await staffRepository.CreateStaff(
            restaurantKey,
            masterKey,
            displayName: $"GENERATED.display-name.{unique}",
            ct);

        var staff = result.Value;

        return staff;
    }

    public async Task<RestaurantStaff> SeedRestaurantStaff(
        RestaurantKey restaurantKey,
        MasterUserKey masterKey,
        params IEnumerable<RoleKey> roleKeys)
    {
        var staffRepository = scope.ServiceProvider
            .GetRequiredService<RestaurantStaffRepository>();;

        var staff = await SeedRestaurantStaff(restaurantKey, masterKey);

        await staffRepository.SetStaffRoles(
            staff, roleKeys, ct);

        return staff;
    }

    public async Task<RestaurantStaff> SeedRestaurantStaff(
        RestaurantKey restaurantKey,
        MasterUserKey masterKey,
        params IEnumerable<PermissionKey> permissionKeys)
    {
        var role = await SeedRole(
            restaurantKey, permissionKeys);

        var staff = await SeedRestaurantStaff(
            restaurantKey, masterKey, role);

        return staff;
    }

    public async Task<WorkerUser> SeedWorker(
        BranchKey branchKey,
        params IEnumerable<RoleKey> roleKeys)
    {
        var unique = TestSeedingGenerator.GetUniqueString();

        var workerRepository = scope.ServiceProvider
            .GetRequiredService<WorkerRepository>();

        var result = await workerRepository.CreateWorker(
            branchKey,
            name: $"GENERATED.role-name.{unique}",
            phone: TestSeedingGenerator.GetPhone(),
            ct: ct);

        var worker = result.Value;

        await workerRepository.SetRoles(
            worker, roleKeys, ct);

        return result.Value;
    }

    public async Task<WorkerUser> SeedWorker(
        BranchKey branchKey,
        params IEnumerable<PermissionKey> permissionKeys)
    {
        var role = await SeedRole(
            new(branchKey.RestaurantId), permissionKeys);

        var worker = await SeedWorker(branchKey, role);

        return worker;
    }

    public async Task<Bill> SeedBill(
        TableKey tableKey,
        short? pax = null,
        ConsumerUserKey? consumerKey = null)
    {
        var billRepository = scope.ServiceProvider
            .GetRequiredService<BillRepository>();

        var result = await billRepository.CreateBill(
            tableKey,
            pax,
            consumerKey,
            ct: ct);

        return result.Value;
    }

    public async Task<Order> SeedOrder(
        Bill bill,
        params IEnumerable<(Menu, short)> items)
    {
        var unique = TestSeedingGenerator.GetUniqueString();

        var orderRepository = scope.ServiceProvider
            .GetRequiredService<OrderRepository>();

        var orderResult = await orderRepository.CreateOrder(bill);

        var order = orderResult.Value;

        foreach (var (menu, quantity) in items)
        {
            var itemResult = await orderRepository.CreateItem(
                order, menu,
                quantity,
                note: $"GENERATED.order-item-note.{unique}",
                ct);

            // result in duplicate item (x2)
            // order.Items.Add(item);
        }

        return order;
    }

    public async Task<Order> SeedOrder(
        Bill bill, IEnumerable<Menu> menus)
    {
        var MenuPool = menus.ToArray();

        var items = Random.Shared.GetItems(MenuPool, Random.Shared.Next(5))
                .Select(menu =>
                    (menu, (short)Random.Shared.Next(1, 5)));

        var order = await SeedOrder(bill, items);

        return order;
    }

    public async Task<ICollection<Order>> SeedOrders(
        Bill bill, IEnumerable<Menu> menus)
    {
        var menuPool = menus.ToArray();
        var orders = new List<Order>();

        foreach (var _ in Enumerable.Range(0, Random.Shared.Next(10)))
        {
            var order = await SeedOrder(bill, menuPool);

            orders.Add(order);
        }

        return orders;
    }

    public async Task<Tag> SeedTag(RestaurantKey restaurantKey, string? type = null)
    {
        var unique = TestSeedingGenerator.GetUniqueString();

        var tagRepository = scope.ServiceProvider
            .GetRequiredService<TagRepository>();

        var result = await tagRepository.CreateTag(
            restaurantKey,
            name: $"GENERATED.tag-name.{unique}",
            type: type,
            ct: ct);

        return result.Value;
    }

    public async Task<ICollection<Tag>> SeedTags(RestaurantKey restaurantKey)
    {
        var tags = new List<Tag>();

        for (var i = 0; i < Random.Shared.Next(6); i++)
        {
            var tag = await SeedTag(restaurantKey);

            tags.Add(tag);
        }

        return tags;
    }

    public async Task<StockTransaction> SeedStockTransaction(
        BranchKey branchKey, IngredientKey ingredientKey, decimal? amount = null)
    {
        var unique = TestSeedingGenerator.GetUniqueString();

        var stockRepository = scope.ServiceProvider
            .GetRequiredService<StockTransactionRepository>();

        var result = await stockRepository.CreateTransaction(
            branchKey, ingredientKey, null,
            amount: amount ?? Random.Shared.Next(0, 100),
            note: $"GENERATED.stock-transaction-note.{unique}",
            ct: ct);

        return result.Value;
    }

    public async Task<BillMember> SeedBillMember(
        BillKey billKey,
        ConsumerUserKey? consumerKey = null)
    {
        var faker = new Faker();

        var memberRepository = scope.ServiceProvider
            .GetRequiredService<BillMemberRepository>();

        var result = await memberRepository.CreateMember(
            billKey: billKey,
            name: faker.Name.FirstName(),
            consumerKey: consumerKey,
            ct: ct);

        return result.Value;
    }

    public async Task<OrderingPortal> SeedOrderingPortal(
        BillKey billKey)
    {
        var portalRepository = scope.ServiceProvider
            .GetRequiredService<OrderingPortalRepository>();

        var result = await portalRepository.CreatePortal(
            billKey: billKey,
            maxUsage: 1,
            validDuration: TimeSpan.FromHours(1),
            ct: ct);

        return result.Value;
    }
}