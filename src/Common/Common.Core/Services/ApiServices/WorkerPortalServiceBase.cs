namespace FoodSphere.Common.Service;

public class WorkerPortalServiceBase(
    PersistenceService persistenceService,
    WorkerPortalRepository portalRepository
) : ServiceBase
{
    public async Task<TDto[]> ListPortals<TDto>(
        Expression<Func<WorkerPortal, TDto>> projection,
        Expression<Func<WorkerPortal, bool>> predicate,
        CancellationToken ct = default)
    {
        return await portalRepository.QueryPortals()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<TDto?> GetPortal<TDto>(
        Expression<Func<WorkerPortal, TDto>> projection, WorkerUserKey key,
        CancellationToken ct = default)
    {
        return await portalRepository.QueryPortals()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.BranchId == key.BranchId &&
                e.WorkerId == key.Id)
            .OrderByDescending(e => e.Id)
            .Select(projection)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<ResultObject<(PortalKey, TDto)>> SetPortal<TDto>(
        Expression<Func<WorkerPortal, TDto>> projection,
        WorkerPortalCreateCommand command,
        CancellationToken ct = default)
    {
        var result = await portalRepository.CreatePortal(
            workerKey: command.WorkerKey,
            validDuration: command.ValidDuration,
            ct: ct);

        if (!result.TryGetValue(out var portal))
            return result.Errors;

        await persistenceService.Commit(ct);

        var deleted = await portalRepository.QueryPortals()
            .Where(e =>
                e.RestaurantId == command.WorkerKey.RestaurantId &&
                e.BranchId == command.WorkerKey.BranchId &&
                e.WorkerId == command.WorkerKey.Id &&
                e.Id != portal.Id)
            .ExecuteDeleteAsync(ct);

        var response = await portalRepository.QuerySinglePortal(portal)
            .Select(projection)
            .SingleOrDefaultAsync(ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created portal.");

        return (portal, response);
    }
}