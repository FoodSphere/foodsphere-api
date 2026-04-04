namespace FoodSphere.Common.Service;

public class BranchStaffServiceBase(
    PersistenceService persistenceService,
    BranchStaffRepository staffRepository
) : ServiceBase
{
    public async Task<TDto[]> ListStaffs<TDto>(
        Expression<Func<BranchStaff, TDto>> projection,
        Expression<Func<BranchStaff, bool>> predicate,
        CancellationToken ct = default)
    {
        return await staffRepository.QueryStaffs()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(BranchStaffKey, TDto)>> CreateStaff<TDto>(
        Expression<Func<BranchStaff, TDto>> projection,
        BranchStaffCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await staffRepository.CreateStaff(
            branchKey: command.BranchKey,
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
                "Failed to retrieve the created branch's staff.");

        return (staff, response);
    }

    public async Task<TDto?> GetStaff<TDto>(
        Expression<Func<BranchStaff, TDto>> projection, BranchStaffKey key,
        CancellationToken ct = default)
    {
        return await staffRepository.QuerySingleStaff(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateStaff(
        BranchStaffKey key, BranchStaffUpdateCommand command,
        CancellationToken ct = default)
    {
        var staff = await staffRepository.GetStaff(key, ct);

        if (staff is null)
            return ResultObject.Fail(ResultError.NotFound,
                "BranchStaff not found.");

        staff.DisplayName = command.DisplayName;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteStaff(
        BranchStaffKey key, CancellationToken ct = default)
    {
        var deleteResult = await staffRepository.DeleteStaff(key, ct);

        if (deleteResult.IsFailed)
            return deleteResult.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> SetStaffRoles(
        BranchStaffKey key,
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