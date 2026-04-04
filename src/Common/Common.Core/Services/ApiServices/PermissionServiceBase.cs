namespace FoodSphere.Common.Service;

public class PermissionServiceBase(
    PersistenceService persistenceService,
    PermissionRepository permissionRepository
) : ServiceBase
{
    public async Task<TDto[]> ListPermissions<TDto>(
        Expression<Func<Permission, TDto>> projection,
        Expression<Func<Permission, bool>> predicate,
        CancellationToken ct = default)
    {
        return await permissionRepository.QueryPermissions()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }
}