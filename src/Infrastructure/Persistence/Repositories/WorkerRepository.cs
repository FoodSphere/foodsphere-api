namespace FoodSphere.Infrastructure.Repository;

public class WorkerRepository(
    FoodSphereDbContext context
) : RepositoryBase(context)
{
    public async Task<ResultObject<WorkerUser>> CreateWorker(
        BranchKey branchKey,
        string name,
        string? phone,
        CancellationToken ct = default)
    {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<WorkerUser>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.RestaurantId == branchKey.RestaurantId &&
                e.Entity.BranchId == branchKey.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<WorkerUser>().Local
                .Where(e =>
                    e.RestaurantId == branchKey.RestaurantId &&
                    e.BranchId == branchKey.Id)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<WorkerUser>()
                .Where(e =>
                    e.RestaurantId == branchKey.RestaurantId &&
                    e.BranchId == branchKey.Id)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var worker = new WorkerUser
        {
            Id = (short)(lastId + 1),
            RestaurantId = branchKey.RestaurantId,
            BranchId = branchKey.Id,
            Name = name,
            Phone = phone,
        };

        _ctx.Add(worker);

        return worker;
    }

    WorkerUser CreateWorkerStub(WorkerUserKey key)
    {
        var worker = new WorkerUser
        {
            RestaurantId = key.RestaurantId,
            BranchId = key.BranchId,
            Id = key.Id,
            Name = default!,
        };

        _ctx.Attach(worker);

        return worker;
    }

    public IQueryable<WorkerUser> QueryWorkers()
    {
        return _ctx.Set<WorkerUser>()
            .AsExpandable();
    }

    public IQueryable<WorkerUser> QuerySingleWorker(WorkerUserKey key)
    {
        return QueryWorkers()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.BranchId == key.BranchId &&
                e.Id == key.Id);
    }

    public async Task<WorkerUser?> GetWorker(
        WorkerUserKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<WorkerUser>(key, ct);
    }

    public async Task<ResultObject> SetRoles(
        WorkerUserKey key,
        IEnumerable<RoleKey> roleKeys,
        CancellationToken ct = default)
    {
        var desired = roleKeys
            .Distinct()
            .ToArray();

        var existed = await _ctx.Set<WorkerRole>()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.BranchId == key.BranchId &&
                e.WorkerId == key.Id)
            .ToArrayAsync(ct);

        var toRemove = existed
            .ExceptBy(desired.Select(e => e.Id), e => e.RoleId);

        var toAdd = desired
            .ExceptBy(existed.Select(e => e.RoleId), e => e.Id)
            .Select(roleKey => new WorkerRole
            {
                RestaurantId = key.RestaurantId,
                BranchId = key.BranchId,
                WorkerId = key.Id,
                RoleId = roleKey.Id,
            });

        _ctx.RemoveRange(toRemove);
        _ctx.AddRange(toAdd);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteWorker(
        WorkerUserKey key, CancellationToken ct = default)
    {
        var worker = await GetWorker(key, ct);

        if (worker is null)
            return ResultObject.NotFound(key);

        // await SetRoles(key, [], ct);
        _ctx.Remove(worker);

        return ResultObject.Success();
    }
}