namespace FoodSphere.Infrastructure.Repository;

public class TableRepository(
    FoodSphereDbContext context,
    IPublishEndpoint publishEndpoint
) : RepositoryBase(context)
{
    public async Task<ResultObject<Table>> CreateTable(
        BranchKey branchKey,
        string? name,
        CancellationToken ct = default)
    {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Table>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.RestaurantId == branchKey.RestaurantId &&
                e.Entity.BranchId == branchKey.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Table>().Local
                .Where(e =>
                    e.RestaurantId == branchKey.RestaurantId &&
                    e.BranchId == branchKey.Id)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Table>()
                .Where(e =>
                    e.RestaurantId == branchKey.RestaurantId &&
                    e.BranchId == branchKey.Id)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var table = new Table
        {
            Id = (short)(lastId + 1),
            RestaurantId = branchKey.RestaurantId,
            BranchId = branchKey.Id,
            Name = name
        };

        _ctx.Add(table);

        await publishEndpoint.Publish(
            new TableCreatedMessage
            {
                Resource = new(
                    branchKey.RestaurantId, branchKey.Id, table.Id),
            },
            ct);

        return table;
    }

    Table CreateTableStub(TableKey key)
    {
        var table = new Table
        {
            Id = key.Id,
            RestaurantId = key.RestaurantId,
            BranchId = key.BranchId,
        };

        _ctx.Attach(table);

        return table;
    }

    public IQueryable<Table> QueryTables()
    {
        return _ctx.Set<Table>()
            .AsExpandable();
    }

    public IQueryable<Table> QuerySingleTable(TableKey key)
    {
        return QueryTables()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.BranchId == key.BranchId &&
                e.Id == key.Id);
    }

    public async Task<Table?> GetTable(
        TableKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<Table>(key, ct);
    }

    public async Task<ResultObject> UpdateTableStatus(
        TableKey key, TableStatus status,
        CancellationToken ct = default)
    {
        var table = await GetTable(key, ct);

        if (table is null)
            return ResultObject.NotFound(key);

        table.Status = status;

        await publishEndpoint.Publish(
            new TableStatusUpdatedMessage
            {
                Resource = new(key.RestaurantId, key.BranchId, key.Id),
                Status = status,
            },
            ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteTable(
        TableKey key, CancellationToken ct = default)
    {
        var table = await GetTable(key, ct);

        if (table is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(table);

        return ResultObject.Success();
    }
}