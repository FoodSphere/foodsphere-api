namespace FoodSphere.Infrastructure.Repository;

public class WorkerPortalRepository(
    FoodSphereDbContext context
) : RepositoryBase(context)
{
    public async Task<ResultObject<WorkerPortal>> CreatePortal(
        WorkerUserKey workerKey,
        TimeSpan? validDuration = null,
        short maxUsage = 1,
        CancellationToken ct = default)
    {
        var portal = new WorkerPortal
        {
            Id = Guid.NewGuid(),
            RestaurantId = workerKey.RestaurantId,
            BranchId = workerKey.BranchId,
            WorkerId = workerKey.Id,
            ValidDuration = validDuration,
            MaxUsage = maxUsage,
        };

        _ctx.Add(portal);

        return portal;
    }

    WorkerPortal CreatePortalStub(PortalKey key)
    {
        var portal = new WorkerPortal
        {
            Id = key.Id,
            RestaurantId = default!,
            BranchId = default!,
            WorkerId = default!,
        };

        _ctx.Attach(portal);

        return portal;
    }

    public IQueryable<WorkerPortal> QueryPortals()
    {
        return _ctx.Set<WorkerPortal>()
            .AsExpandable();
    }

    public IQueryable<WorkerPortal> QuerySinglePortal(PortalKey key)
    {
        return QueryPortals()
            .Where(e =>
                e.Id == key.Id);
    }

    public async Task<WorkerPortal?> GetPortal(
        PortalKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<WorkerPortal>(key, ct);
    }
}