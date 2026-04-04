namespace FoodSphere.Infrastructure.Repository;

public class RestaurantStaffRepository(
    FoodSphereDbContext context
) : RepositoryBase(context)
{
    public async Task<ResultObject<RestaurantStaff>> CreateStaff(
        RestaurantKey restaurantKey,
        MasterUserKey masterKey,
        string displayName,
        CancellationToken ct = default)
    {
        var staff = new RestaurantStaff
        {
            RestaurantId = restaurantKey.Id,
            MasterId = masterKey.Id,
            DisplayName = displayName,
        };

        _ctx.Add(staff);

        return staff;
    }

    RestaurantStaff CreateStaffStub(RestaurantStaffKey key)
    {
        var staff = new RestaurantStaff
        {
            RestaurantId = key.RestaurantId,
            MasterId = key.MasterId,
            DisplayName = null!,
        };

        _ctx.Attach(staff);

        return staff;
    }

    public IQueryable<RestaurantStaff> QueryStaffs()
    {
        return _ctx.Set<RestaurantStaff>()
            .AsExpandable();
    }

    public IQueryable<RestaurantStaff> QuerySingleStaff(
        RestaurantStaffKey key)
    {
        return QueryStaffs()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.MasterId == key.MasterId);
    }

    public async Task<RestaurantStaff?> GetStaff(
        RestaurantStaffKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<RestaurantStaff>(key, ct);
    }

    public async Task<ResultObject> DeleteStaff(
        RestaurantStaffKey key, CancellationToken ct = default)
    {
        var staff = await GetStaff(key, ct);

        if (staff is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(staff);

        return ResultObject.Success();
    }

    public async Task<ResultObject> SetStaffRoles(
        RestaurantStaffKey key,
        IEnumerable<RoleKey> roleKeys,
        CancellationToken ct = default)
    {
        var desired = roleKeys
            .Distinct()
            .ToArray();

        var existed = await _ctx.Set<RestaurantStaffRole>()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.MasterId == key.MasterId)
            .ToArrayAsync(ct);

        var toRemove = existed
            .ExceptBy(desired.Select(e => e.Id), e => e.RoleId);

        var toAdd = desired
            .ExceptBy(existed.Select(e => e.RoleId), e => e.Id)
            .Select(roleKey => new RestaurantStaffRole
            {
                RestaurantId = key.RestaurantId,
                MasterId = key.MasterId,
                RoleId = roleKey.Id,
            });

        _ctx.RemoveRange(toRemove);
        _ctx.AddRange(toAdd);

        return ResultObject.Success();
    }
}