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
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Branch>()
            .Any(e =>
            e.State == EntityState.Added &&
            e.Entity.RestaurantId == restaurantId);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Branch>().Local
                .Where(e => e.RestaurantId == restaurantId)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Branch>()
                .Where(e => e.RestaurantId == restaurantId)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

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

    public Branch GetBranchStub(Guid restaurantId, short branchId)
    {
        var branch = new Branch
        {
            RestaurantId = restaurantId,
            Id = branchId,
            Name = default!,
        };

        _ctx.Attach(branch);

        return branch;
    }

    public IQueryable<Branch> QueryBranches()
    {
        return _ctx.Set<Branch>()
            .AsExpandable();
    }

    public IQueryable<Branch> QuerySingleBranch(Guid restaurantId, short branchId)
    {
        return QueryBranches()
            .Where(e =>
                e.RestaurantId == restaurantId &&
                e.Id == branchId);
    }

    public async Task<Branch?> GetBranch(Guid restaurantId, short branchId)
    {
        var existed = await _ctx.Set<Branch>()
            .AnyAsync(e =>
                e.RestaurantId == restaurantId &&
                e.Id == branchId);

        if (!existed)
        {
            return null;
        }

        return GetBranchStub(restaurantId, branchId);
    }

    public async Task<TDto?> GetBranch<TDto>(
        Guid restaurantId, short branchId,
        Expression<Func<Branch, TDto>> projection)
    {
        return await QuerySingleBranch(restaurantId, branchId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task SetContact(Branch branch, ContactDto contact)
    {
        if (branch.Contact is null)
        {
            branch.Contact = new Contact();
            _ctx.Add(branch.Contact);
        }

        branch.Contact.Name = contact.name;
        branch.Contact.Email = contact.email;
        branch.Contact.Phone = contact.phone;
    }

    public async Task DeleteContact(Branch branch)
    {
        if (branch.Contact is not null)
        {
            _ctx.Remove(branch.Contact);
        }
    }

    public async Task DeleteBranch(Branch branch)
    {
        _ctx.Remove(branch);
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
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Table>()
            .Any(e =>
            e.State == EntityState.Added &&
            e.Entity.RestaurantId == restaurantId &&
            e.Entity.BranchId == branchId);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Table>().Local
                .Where(e => e.RestaurantId == restaurantId && e.BranchId == branchId)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Table>()
                .Where(e => e.RestaurantId == restaurantId && e.BranchId == branchId)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

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

    public Table GetTableStub(Guid restaurantId, short branchId, short tableId)
    {
        var table = new Table
        {
            Id = tableId,
            RestaurantId = restaurantId,
            BranchId = branchId,
        };

        _ctx.Attach(table);

        return table;
    }

    public IQueryable<Table> QueryTables()
    {
        return _ctx.Set<Table>()
            .AsExpandable();
    }

    public IQueryable<Table> QuerySingleTable(Guid restaurantId, short branchId, short tableId)
    {
        return QueryTables()
            .Where(e =>
                e.RestaurantId == restaurantId &&
                e.BranchId == branchId &&
                e.Id == tableId);
    }

    public async Task<TDto?> GetTable<TDto>(
        Guid restaurantId, short branchId, short tableId,
        Expression<Func<Table, TDto>> projection)
    {
        return await QuerySingleTable(restaurantId, branchId, tableId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<Table?> GetTable(
        Guid restaurantId,
        short branchId,
        short tableId)
    {
        var existed = await _ctx.Set<Table>()
            .AnyAsync(e =>
                e.RestaurantId == restaurantId &&
                e.BranchId == branchId &&
                e.Id == tableId);

        if (!existed)
        {
            return null;
        }

        return GetTableStub(restaurantId, branchId, tableId);
    }

    public async Task DeleteTable(Table table)
    {
        _ctx.Remove(table);
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

    public BranchManager GetManagerStub(Guid restaurantId, short branchId, string masterId)
    {
        var manager = new BranchManager
        {
            RestaurantId = restaurantId,
            BranchId = branchId,
            MasterId = masterId,
        };

        _ctx.Attach(manager);

        return manager;
    }

    public IQueryable<BranchManager> QueryManagers()
    {
        return _ctx.Set<BranchManager>()
            .AsExpandable();
    }

    public IQueryable<BranchManager> QuerySingleManager(Guid restaurantId, short branchId, string masterId)
    {
        return QueryManagers()
            .Where(e =>
                e.RestaurantId == restaurantId &&
                e.BranchId == branchId &&
                e.MasterId == masterId);
    }

    public async Task<TDto?> GetManager<TDto>(
        Guid restaurantId,
        short branchId,
        string masterId,
        Expression<Func<BranchManager, TDto>> projection)
    {
        return await QuerySingleManager(restaurantId, branchId, masterId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<BranchManager?> GetManager(Guid restaurantId, short branchId, string masterId)
    {
        var existed = await _ctx.Set<BranchManager>()
            .AnyAsync(e =>
                e.RestaurantId == restaurantId &&
                e.BranchId == branchId &&
                e.MasterId == masterId);

        if (!existed)
        {
            return null;
        }

        return GetManagerStub(restaurantId, branchId, masterId);
    }

    public async Task SetManagerRoles(
        BranchManager manager,
        IEnumerable<short> roleIds,
        CancellationToken ct = default)
    {
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
        CancellationToken ct = default)
    {
        var desiredIds = roleIds
            .Distinct()
            .ToArray();

        var currentRoles = await _ctx.Set<BranchManagerRole>()
            .Where(bmr =>
                bmr.RestaurantId == restaurantId &&
                bmr.BranchId == branchId &&
                bmr.ManagerId == masterId)
            // .Select(bmr => bmr.RoleId)
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

    public async Task<Stock> CreateStock(Branch branch, short ingredientId, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        var stock = new Stock
        {
            Branch = branch,
            IngredientId = ingredientId,
            Amount = amount
        };

        _ctx.Add(stock);

        return stock;
    }

    public Stock GetStockStub(Guid restaurantId, short branchId, short ingredientId)
    {
        var stock = new Stock
        {
            RestaurantId = restaurantId,
            BranchId = branchId,
            IngredientId = ingredientId,
        };

        _ctx.Attach(stock);

        return stock;
    }

    public IQueryable<Stock> QueryStocks()
    {
        return _ctx.Set<Stock>()
            .AsExpandable();
    }

    public IQueryable<Stock> QuerySingleStock(Guid restaurantId, short branchId, short ingredientId)
    {
        return QueryStocks()
            .Where(e =>
                e.RestaurantId == restaurantId &&
                e.BranchId == branchId &&
                e.IngredientId == ingredientId);
    }

    public async Task<TDto?> GetStock<TDto>(
        Guid restaurantId, short branchId, short ingredientId,
        Expression<Func<Stock, TDto>> projection)
    {
        return await QuerySingleStock(restaurantId, branchId, ingredientId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<Stock?> GetStock(Guid restaurantId, short branchId, short ingredientId)
    {
        var existed = await QuerySingleStock(restaurantId, branchId, ingredientId)
            .AnyAsync();

        if (!existed)
        {
            return null;
        }

        var ingredient = new Ingredient
        {
            Id = ingredientId,
            RestaurantId = restaurantId,
            Name = default!,
        };

        var stock = new Stock
        {
            BranchId = branchId,
            Ingredient = ingredient,
        };

        _ctx.AttachRange(ingredient, stock);

        return stock;
    }

    public async Task<Stock?> GetStock(Branch branch, short ingredientId)
    {
        return await GetStock(branch.RestaurantId, branch.Id, ingredientId);
    }

    public async Task<Stock> SetStock(Branch branch, short ingredientId, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        var item = await GetStock(branch, ingredientId);

        if (item is null)
        {
            item = await CreateStock(branch, ingredientId, amount);
        }
        else
        {
            item.Amount = amount;
        }

        return item;
    }

    public async Task DeleteStock(Stock stock)
    {
        _ctx.Remove(stock);
    }
}