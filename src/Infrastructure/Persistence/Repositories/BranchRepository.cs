namespace FoodSphere.Infrastructure.Repository;

public class BranchRepository(
    FoodSphereDbContext context
) : RepositoryBase(context)
{
    public async Task<ResultObject<Branch>> CreateBranch(
        RestaurantKey restaurantKey,
        string name,
        string? displayName,
        string? address,
        TimeOnly? openingTime,
        TimeOnly? closingTime,
        Contact? contact,
        CancellationToken ct = default)
    {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Branch>()
            .Any(e =>
            e.State == EntityState.Added &&
            e.Entity.RestaurantId == restaurantKey.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Branch>().Local
                .Where(e => e.RestaurantId == restaurantKey.Id)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Branch>()
                .Where(e => e.RestaurantId == restaurantKey.Id)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var branch = new Branch
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantKey.Id,
            Name = name,
            DisplayName = displayName,
            Address = address,
            OpeningTime = openingTime,
            ClosingTime = closingTime,
            Contact = contact ?? new(),
        };

        _ctx.Add(branch);

        return branch;
    }

    Branch CreateBranchStub(BranchKey key)
    {
        var branch = new Branch
        {
            Id = key.Id,
            RestaurantId = key.RestaurantId,
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

    public IQueryable<Branch> QuerySingleBranch(BranchKey key)
    {
        return QueryBranches()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.Id == key.Id);
    }

    public async Task<Branch?> GetBranch(BranchKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<Branch>(key, ct);
    }

    public async Task<ResultObject> DeleteBranch(BranchKey key, CancellationToken ct = default)
    {
        var branch = await GetBranch(key, ct);

        if (branch is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(branch);

        return ResultObject.Success();
    }
}