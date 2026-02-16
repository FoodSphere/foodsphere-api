namespace FoodSphere.Common.Service;

public class StaffService(FoodSphereDbContext context) : ServiceBase(context)
{
    public async Task<StaffUser> CreateStaff(
        Branch branch,
        string name,
        string? phone = null,
        CancellationToken ct = default
    ) {
        return await CreateStaff(
            branch.RestaurantId,
            branch.Id,
            name,
            phone,
            ct);
    }

    public async Task<StaffUser> CreateStaff(
        Guid restaurantId,
        short branchId,
        string name,
        string? phone = null,
        CancellationToken ct = default
    ) {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<StaffUser>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.RestaurantId == restaurantId);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<StaffUser>().Local
                .Where(e =>
                    e.RestaurantId == restaurantId &&
                    e.BranchId == branchId)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<StaffUser>()
                .Where(e =>
                    e.RestaurantId == restaurantId &&
                    e.BranchId == branchId)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var staff = new StaffUser
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantId,
            BranchId = branchId,
            Name = name,
            Phone = phone
        };

        _ctx.Add(staff);

        return staff;
    }

    public StaffUser GetStaffStub(Guid restaurantId, short branchId, short staffId)
    {
        var staff = new StaffUser
        {
            RestaurantId = restaurantId,
            BranchId = branchId,
            Id = staffId,
            Name = default!,
        };

        _ctx.Attach(staff);

        return staff;
    }

    public IQueryable<StaffUser> QueryStaffs()
    {
        return _ctx.Set<StaffUser>()
            .AsExpandable();
    }

    public IQueryable<StaffUser> QuerySingleStaff(Guid restaurantId, short branchId, short staffId)
    {
        return QueryStaffs()
            .Where(e =>
                e.RestaurantId == restaurantId &&
                e.BranchId == branchId &&
                e.Id == staffId);
    }

    public async Task<TDto?> GetStaff<TDto>(
        Guid restaurantId, short branchId, short staffId,
        Expression<Func<StaffUser, TDto>> projection)
    {
        return await QuerySingleStaff(restaurantId, branchId, staffId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<StaffUser?> GetStaff(Guid restaurantId, short branchId, short staffId)
    {
        var existed = QuerySingleStaff(restaurantId, branchId, staffId)
            .AnyAsync();

        if (!await existed)
        {
            return null;
        }

        return GetStaffStub(restaurantId, branchId, staffId);
    }

    public async Task SetRoles(
        StaffUser staff,
        IEnumerable<short> roleIds,
        CancellationToken ct = default
    ) {
        await SetRoles(
            staff.RestaurantId,
            staff.BranchId,
            staff.Id,
            roleIds,
            ct);
    }

    public async Task SetRoles(
        Guid restaurantId,
        short branchId,
        short staffId,
        IEnumerable<short> roleIds,
        CancellationToken ct = default
    ) {
        var desiredIds = roleIds
            .Distinct()
            .ToArray();

        var currentRoles = await _ctx.Set<StaffRole>()
            .Where(sr =>
                sr.RestaurantId == restaurantId &&
                sr.BranchId == branchId &&
                sr.StaffId == staffId)
            .ToArrayAsync(ct);

        var toRemove = currentRoles
            .ExceptBy(desiredIds, sr => sr.RoleId)
            .ToArray();

        var toAddIds = desiredIds
            .Except(currentRoles.Select(sr => sr.RoleId))
            .ToArray();

        var newEntities = toAddIds.Select(roleId => new StaffRole
        {
            RestaurantId = restaurantId,
            BranchId = branchId,
            StaffId = staffId,
            RoleId = roleId
        });

        _ctx.RemoveRange(toRemove);
        await _ctx.AddRangeAsync(newEntities, ct);
    }

    public async Task DeleteStaff(StaffUser staff)
    {
        _ctx.Remove(staff);
    }
}