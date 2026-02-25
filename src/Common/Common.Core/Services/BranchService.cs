using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Service;

public class BranchService(FoodSphereDbContext context) : ServiceBase(context)
{
    public async Task<Branch> CreateBranch(
        Guid restaurantId,
        string name,
        string? displayName = null,
        string? address = null,
        TimeOnly? openingTime = null,
        TimeOnly? closingTime = null,
        CancellationToken ct = default
    ) {
        var lastId = await _ctx.Set<Branch>()
            .Where(branch => branch.RestaurantId == restaurantId)
            .MaxAsync(branch => (int?)branch.Id, ct) ?? 0;

        var branch = new Branch
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantId,
            Name = name,
            DisplayName = displayName,
            Address = address,
            OpeningTime = openingTime,
            ClosingTime = closingTime,
        };

        _ctx.Add(branch);

        return branch;
    }

    public async Task<BranchManager> CreateManager(Guid restaurantId, short branchId, string masterId)
    {
        var manager = new BranchManager
        {
            RestaurantId = restaurantId,
            BranchId = branchId,
            MasterId = masterId,
        };

        _ctx.Add(manager);

        return manager;
    }

    public async Task SetManagerRoles(
        BranchManager manager,
        IEnumerable<short> roleIds,
        CancellationToken ct = default
    ) {
        await SetManagerRoles(
            manager.RestaurantId,
            manager.BranchId,
            manager.MasterId,
            roleIds,
            ct);
    }

    public async Task SetManagerRoles(
        Guid restaurantId,
        short branchId,
        string masterId,
        IEnumerable<short> roleIds,
        CancellationToken ct = default
    ) {
        var desiredIds = roleIds
            .Distinct()
            .ToArray();

        var currentRoles = await _ctx.Set<BranchManagerRole>()
            .Where(bmr =>
                bmr.RestaurantId == restaurantId &&
                bmr.BranchId == branchId &&
                bmr.ManagerId == masterId)
            .ToArrayAsync(ct);

        var toRemove = currentRoles
            .ExceptBy(desiredIds, sr => sr.RoleId)
            .ToArray();

        var toAddIds = desiredIds
            .Except(currentRoles.Select(sr => sr.RoleId))
            .ToArray();

        var newEntities = toAddIds.Select(roleId => new BranchManagerRole
        {
            RestaurantId = restaurantId,
            BranchId = branchId,
            ManagerId = masterId,
            RoleId = roleId
        });

        _ctx.RemoveRange(toRemove);
        await _ctx.AddRangeAsync(newEntities, ct);
    }

    public async Task<Table?> GetTable(
        Guid restaurantId,
        short branchId,
        short tableId
    ) {
        return await _ctx.FindAsync<Table>(restaurantId, branchId, tableId);
    }

    public async Task<Table> CreateTable(
        Branch branch,
        string? name = null
    ) {
        return await CreateTable(
            restaurantId: branch.RestaurantId,
            branchId: branch.Id,
            name: name
        );
    }

    public async Task<Table> CreateTable(
        Guid restaurantId,
        short branchId,
        string? name = null,
        CancellationToken ct = default
    ) {
        var lastId = await _ctx.Set<Table>()
            .Where(table => table.RestaurantId == restaurantId && table.BranchId == branchId)
            .MaxAsync(table => (int?)table.Id, ct) ?? 0;

        var table = new Table
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantId,
            BranchId = branchId,
            Name = name
        };

        _ctx.Add(table);

        return table;
    }

    public async Task<List<Table>> ListTables(Guid restaurantId, short branchId)
    {
        return await _ctx.Set<Table>()
            .Where(table => table.RestaurantId == restaurantId && table.BranchId == branchId)
            .ToListAsync();
    }

    public async Task<List<BranchManager>> ListManagers(Guid restaurantId, short branchId)
    {
        return await _ctx.Set<BranchManager>()
            .Where(manager => manager.RestaurantId == restaurantId && manager.BranchId == branchId)
            .ToListAsync();
    }

    public async Task<Branch?> GetBranch(Guid restaurantId, short branchId)
    {
        return await _ctx.FindAsync<Branch>(restaurantId, branchId);
    }

    public async Task<List<Branch>> ListBranches(Guid restaurantId)
    {
        return await _ctx.Set<Branch>()
            .Where(branch => branch.RestaurantId == restaurantId)
            .ToListAsync();
    }

    public async Task DeleteBranch(Branch branch)
    {
        _ctx.Remove(branch);
    }

    public async Task DeleteTable(Table table)
    {
        _ctx.Remove(table);
    }

    public async Task<Stock?> GetStock(Guid restaurantId, short branchId, short ingredientId)
    {
        return await _ctx.FindAsync<Stock>(restaurantId, branchId, ingredientId);
    }

    public async Task<Stock?> GetStock(Branch branch, short ingredientId)
    {
        return await GetStock(branch.RestaurantId, branch.Id, ingredientId);
    }

    public async Task SetStock(Branch branch, short ingredientId, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        var item = await GetStock(branch, ingredientId);

        if (item is null)
        {
            if (amount == 0)
            {
                return;
            }
            else
            {
                item = new Stock
                {
                    RestaurantId = branch.RestaurantId,
                    BranchId = branch.Id,
                    IngredientId = ingredientId,
                    Amount = amount
                };

                _ctx.Add(item);
            }
        }
        else
        {
            item.Amount = amount;
            // _ctx.Entry(item).State = EntityState.Modified;
        }
    }

    public async Task DeleteStock(Stock stock)
    {
        _ctx.Remove(stock);
    }

    public async Task SetContact(Branch branch, ContactDto contact)
    {
        if (branch.Contact is null)
        {
            branch.Contact = new Contact();
            _ctx.Add(branch.Contact);
        }

        branch.Contact.Name = contact?.name;
        branch.Contact.Email = contact?.email;
        branch.Contact.Phone = contact?.phone;
    }

    public async Task DeleteContact(Branch branch)
    {
        if (branch.Contact is not null)
        {
            _ctx.Remove(branch.Contact);
        }
    }
}