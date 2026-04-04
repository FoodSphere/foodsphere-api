namespace FoodSphere.Common.Service;

public class RestaurantStaffServiceBase(
    PersistenceService persistenceService,
    RestaurantStaffRepository staffRepository,
    RoleRepository roleRepository
) : ServiceBase
{
    public async Task<TDto[]> ListStaffs<TDto>(
        Expression<Func<RestaurantStaff, TDto>> projection,
        Expression<Func<RestaurantStaff, bool>> predicate,
        CancellationToken ct = default)
    {
        return await staffRepository.QueryStaffs()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(RestaurantStaffKey, TDto)>> CreateStaff<TDto>(
        Expression<Func<RestaurantStaff, TDto>> projection,
        RestaurantStaffCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await staffRepository.CreateStaff(
            restaurantKey: command.RestaurantKey,
            masterKey: command.MasterKey,
            displayName: command.DisplayName,
            ct);

        if (!createResult.TryGetValue(out var staff))
            return createResult.Errors;

        var roleResult = await staffRepository.SetStaffRoles(
            staff, command.RoleKeys, ct);

        if (roleResult.IsFailed)
            return roleResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetStaff(projection, staff, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created restaurant's staff.");

        return (staff, response);
    }

    public async Task<TDto?> GetStaff<TDto>(
        Expression<Func<RestaurantStaff, TDto>> projection, RestaurantStaffKey key,
        CancellationToken ct = default)
    {
        return await staffRepository.QuerySingleStaff(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateStaff(
        RestaurantStaffKey key, RestaurantStaffUpdateCommand command,
        CancellationToken ct = default)
    {
        var staff = await staffRepository.GetStaff(key, ct);

        if (staff is null)
            return ResultObject.Fail(ResultError.NotFound,
                "RestaurantStaff not found.");

        staff.DisplayName = command.DisplayName;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteStaff(
        RestaurantStaffKey key, CancellationToken ct = default)
    {
        var result = await staffRepository.DeleteStaff(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> SetStaffRoles(
        RestaurantStaffKey key,
        IEnumerable<RoleKey> roleKeys,
        CancellationToken ct = default)
    {
        var result = await staffRepository.SetStaffRoles(
            key, roleKeys, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}