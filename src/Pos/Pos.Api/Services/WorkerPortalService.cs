using FoodSphere.Infrastructure.Repository;

namespace FoodSphere.Pos.Api.Service;

#pragma warning disable CS9107
public class WorkerPortalService(
    PersistenceService persistenceService,
    WorkerAuthService workerAuthService,
    WorkerPortalRepository portalRepository
) : WorkerPortalServiceBase(
    persistenceService,
    portalRepository)
{
    public async Task<ResultObject<string>> GenerateToken(
        PortalKey key, CancellationToken ct = default)
    {
        var portal = await portalRepository.GetPortal(key, ct);

        if (portal is null)
            return ResultObject.NotFound(key);

        if (portal.IsUnusable)
            return ResultObject.Fail(ResultError.State,
                "The portal is unusable.");

        portal.Use();

        var token = await workerAuthService.GenerateToken(
            new(portal.RestaurantId, portal.BranchId, portal.WorkerId));

        await persistenceService.Commit(ct);

        return token;
    }
}