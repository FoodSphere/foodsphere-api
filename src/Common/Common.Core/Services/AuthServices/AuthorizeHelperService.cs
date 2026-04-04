namespace FoodSphere.Common.Service;

public class AuthorizeHelperService(
    FoodSphereDbContext context
) : ServiceBase
{
    public async Task<bool> IsRestaurantOwner(
        RestaurantKey restaurantKey,
        MasterUserKey masterKey)
    {
        return await context.Set<Restaurant>()
            .AnyAsync(e =>
                e.Id == restaurantKey.Id &&
                e.OwnerId == masterKey.Id);
    }

    public async Task<bool> IsRestaurantStaff(
        RestaurantKey restaurantKey,
        MasterUserKey masterKey)
    {
        return await context.Set<RestaurantStaff>()
            .AnyAsync(e =>
                e.RestaurantId == restaurantKey.Id &&
                e.MasterId == masterKey.Id);
    }

    public async Task<bool> IsBranchStaff(
        Guid? restaurantId,
        short? branchId,
        string? masterUserId)
    {
        return await context.Set<BranchStaff>()
            .AnyAsync(e =>
                e.RestaurantId == restaurantId &&
                e.BranchId == branchId &&
                e.MasterId == masterUserId);
    }

    IQueryable<Role> QueryRestaurantStaffRoles(RestaurantStaffKey key)
    {
        return context.Set<RestaurantStaffRole>()
            .Where(e =>
                e.MasterId == key.MasterId &&
                e.RestaurantId == key.RestaurantId)
            .Select(e => e.Role);
    }

    public async Task<TDto[]> ListStaffRoles<TDto>(
        RestaurantStaffKey key,
        Expression<Func<Role, TDto>> projection,
        CancellationToken ct = default)
    {
        return await QueryRestaurantStaffRoles(key)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<TDto[]> ListStaffPermissions<TDto>(
        RestaurantStaffKey key,
        Expression<Func<Permission, TDto>> projection,
        CancellationToken ct = default)
    {
        return await QueryRestaurantStaffRoles(key)
            .SelectMany(r => r.Permissions)
            .Select(rp => rp.Permission)
            .GroupBy(p => p.Id)
            .Select(g => g.First())
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<bool> CheckRestaurantStaffPermission(
        RestaurantStaffKey key,
        params int[] permissionIds)
    {
        var count = await QueryRestaurantStaffRoles(key)
            .SelectMany(r => r.Permissions)
            .Where(rp => permissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .Distinct()
            .CountAsync();

        return count == permissionIds.Length;
    }

    public async Task<Permission[]> GetMissingRestaurantStaffPermissions(
        RestaurantStaffKey key,
        params Permission[] requiredPermissions)
    {
        var requiredIds = requiredPermissions.Select(p => p.Id)
            .Distinct().ToArray();

        var userPermissions = await QueryRestaurantStaffRoles(key)
            .SelectMany(r => r.Permissions)
            .Where(rp => requiredIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .Distinct()
            .ToArrayAsync();

        return requiredPermissions
            .ExceptBy(userPermissions, e => e.Id)
            .ToArray();
    }

    public async Task<bool> CheckBranchStaffPermission(
        Guid? restaurantId,
        short? branchId,
        string? masterUserId,
        params int[] permissionIds)
    {
        var count = await context.Set<BranchStaff>()
            .Where(bm =>
                bm.RestaurantId == restaurantId &&
                bm.BranchId == branchId &&
                bm.MasterId == masterUserId)
            .SelectMany(m => m.Roles)
            .Select(mr => mr.Role) // extra join via navigation property
            .SelectMany(r => r.Permissions)
            .Where(rp => permissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .Distinct()
            .CountAsync();

        return count == permissionIds.Length;
    }

    IQueryable<Role> QueryWorkerRoles(
        WorkerUserKey key)
    {
        return context.Set<WorkerRole>()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.BranchId == key.BranchId &&
                e.WorkerId == key.Id)
            .Select(e => e.Role);
    }

    public async Task<TDto[]> ListWorkerRoles<TDto>(
        WorkerUserKey key,
        Expression<Func<Role, TDto>> projection,
        CancellationToken ct = default)
    {
        return await QueryWorkerRoles(key)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<TDto[]> ListWorkerPermissions<TDto>(
        WorkerUserKey key,
        Expression<Func<Permission, TDto>> projection,
        CancellationToken ct = default)
    {
        return await QueryWorkerRoles(key)
            .SelectMany(r => r.Permissions)
            .Select(rp => rp.Permission)
            .GroupBy(p => p.Id)
            .Select(g => g.First())
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<bool> CheckWorkerPermission(
        WorkerUserKey key,
        params int[] permissionIds)
    {
        var count = await QueryWorkerRoles(key)
            .SelectMany(r => r.Permissions)
            .Where(rp => permissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .Distinct()
            .CountAsync();

        return count == permissionIds.Length;
    }

    public async Task<Permission[]> GetMissingWorkerPermissions(
        WorkerUserKey key,
        params Permission[] requiredPermissions)
    {
        var requiredIds = requiredPermissions.Select(p => p.Id)
            .Distinct().ToArray();

        var userPermissions = await QueryWorkerRoles(key)
            .SelectMany(r => r.Permissions)
            .Where(rp => requiredIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .Distinct()
            .ToArrayAsync();

        return requiredPermissions
            .ExceptBy(userPermissions, e => e.Id)
            .ToArray();
    }
}