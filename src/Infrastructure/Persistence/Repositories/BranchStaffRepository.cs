namespace FoodSphere.Infrastructure.Repository;

public class BranchStaffRepository(
    FoodSphereDbContext context
) : RepositoryBase(context)
{
    public async Task<ResultObject<BranchStaff>> CreateStaff(
        BranchKey branchKey,
        MasterUserKey masterKey,
        string displayName,
        CancellationToken ct = default)
    {
        var staff = new BranchStaff
        {
            RestaurantId = branchKey.RestaurantId,
            BranchId = branchKey.Id,
            MasterId = masterKey.Id,
            DisplayName = displayName,
        };

        _ctx.Add(staff);

        return staff;
    }

    public IQueryable<BranchStaff> QueryStaffs()
    {
        return _ctx.Set<BranchStaff>()
            .AsExpandable();
    }

    public IQueryable<BranchStaff> QuerySingleStaff(
        BranchStaffKey key)
    {
        return QueryStaffs()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.BranchId == key.BranchId &&
                e.MasterId == key.MasterId);
    }

    public async Task<BranchStaff?> GetStaff(
        BranchStaffKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<BranchStaff>(key, ct);
    }

    public async Task<ResultObject> DeleteStaff(
        BranchStaffKey key, CancellationToken ct = default)
    {
        var staff = await GetStaff(key, ct);

        if (staff is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(staff);

        return ResultObject.Success();
    }

    public async Task<ResultObject> SetStaffRoles(
        BranchStaffKey key,
        IEnumerable<RoleKey> roleKeys,
        CancellationToken ct = default)
    {
        var desired = roleKeys
            .Distinct()
            .ToArray();

        var existed = await _ctx.Set<BranchStaffRole>()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.BranchId == key.BranchId &&
                e.MasterId == key.MasterId)
            .ToArrayAsync(ct);

        var toRemove = existed
            .ExceptBy(desired.Select(e => e.Id), e => e.RoleId);

        var toAdd = desired
            .ExceptBy(existed.Select(e => e.RoleId), e => e.Id)
            .Select(roleKey => new BranchStaffRole
            {
                RestaurantId = key.RestaurantId,
                BranchId = key.BranchId,
                MasterId = key.MasterId,
                RoleId = roleKey.Id,
            });

        _ctx.RemoveRange(toRemove);
        _ctx.AddRange(toAdd);

        return ResultObject.Success();
    }
}