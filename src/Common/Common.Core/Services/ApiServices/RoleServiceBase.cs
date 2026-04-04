namespace FoodSphere.Common.Service;

public class RoleServiceBase(
    PersistenceService persistenceService,
    RoleRepository roleRepository
) : ServiceBase
{
    public async Task<TDto[]> ListRoles<TDto>(
        Expression<Func<Role, TDto>> projection,
        Expression<Func<Role, bool>> predicate,
        CancellationToken ct = default)
    {
        return await roleRepository.QueryRoles()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(RoleKey, TDto)>> CreateRole<TDto>(
        Expression<Func<Role, TDto>> projection,
        RoleCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await roleRepository.CreateRole(
            restaurantKey: command.RestaurantKey,
            name: command.Name,
            description: command.Description,
            ct);

        if (!createResult.TryGetValue(out var role))
            return createResult.Errors;

        var permissionResult = await roleRepository.SetPermissions(
            role, command.PermissionKeys, ct);

        if (permissionResult.IsFailed)
            return permissionResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetRole(projection, role, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created role.");

        return (role, response);
    }

    public async Task<TDto?> GetRole<TDto>(
        Expression<Func<Role, TDto>> projection, RoleKey key,
        CancellationToken ct = default)
    {
        return await roleRepository.QuerySingleRole(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateRole(
        RoleKey key, RoleUpdateCommand command,
        CancellationToken ct = default)
    {
        var role = await roleRepository.GetRole(key, ct);

        if (role is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Role not found.");

        role.Name = command.Name;
        role.Description = command.Description;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteRole(
        RoleKey key, CancellationToken ct = default)
    {
        var result = await roleRepository.DeleteRole(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> SetPermissions(
        RoleKey key, IEnumerable<PermissionKey> permissionKeys,
        CancellationToken ct = default)
    {
        var result = await roleRepository.SetPermissions(
            key, permissionKeys, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}