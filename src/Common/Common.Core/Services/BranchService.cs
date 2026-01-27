using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Services;

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

        await _ctx.AddAsync(branch, ct);

        return branch;
    }

    public async Task<Manager> CreateManager(Guid restaurantId, short branchId, string masterId)
    {
        var manager = new Manager
        {
            RestaurantId = restaurantId,
            BranchId = branchId,
            MasterId = masterId,
        };

        await _ctx.AddAsync(manager);

        return manager;
    }

    public async Task UpdateManagerPermissions(Manager manager, List<short> roleIds)
    {

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

        await _ctx.AddAsync(table, ct);

        return table;
    }

    public async Task<List<Table>> ListTables(Guid restaurantId, short branchId)
    {
        return await _ctx.Set<Table>()
            .Where(table => table.RestaurantId == restaurantId && table.BranchId == branchId)
            .ToListAsync();
    }

    public async Task<List<StaffUser>> ListStaffs(Guid restaurantId, short branchId)
    {
        return await _ctx.Set<StaffUser>()
            .Where(staff => staff.RestaurantId == restaurantId && staff.BranchId == branchId)
            .ToListAsync();
    }

    public async Task<List<Manager>> ListManagers(Guid restaurantId, short branchId)
    {
        return await _ctx.Set<Manager>()
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
            await _ctx.AddAsync(branch.Contact);
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

    public async Task<bool> CheckPermissions(Branch branch, MasterUser user, Permission[]? permissions = null)
    {
        return await _ctx.Set<Restaurant>()
            .AnyAsync(r =>
                r.Id == branch.RestaurantId && (
                    r.OwnerId == user.Id ||
                    _ctx.Set<Manager>().Any(m =>
                        m.RestaurantId == r.Id &&
                        m.MasterId == user.Id &&
                        m.BranchId == branch.Id
            )));
    }

    public async Task<bool> CheckPermissions(Branch branch, StaffUser user, Permission[]? permissions = null)
    {
        return branch.RestaurantId == user.RestaurantId &&
               branch.Id == user.BranchId;
    }
}