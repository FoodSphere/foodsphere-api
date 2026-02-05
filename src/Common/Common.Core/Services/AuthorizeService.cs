using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Services;

public class AuthorizeService(
    FoodSphereDbContext context
) : ServiceBase(context)
{
    // public async Task<Permission[]?> GetPermissions(
    //     MasterUser user,
    //     Guid restaurantId,
    //     short branchId
    // ) {
    //     var restaurant = await _ctx.Set<Restaurant>()
    //         .FindAsync(restaurantId)
    //         ?? throw new InvalidOperationException();

    //     if (restaurant.OwnerId == user.Id)
    //     {
    //         return [.. _ctx.Set<Permission>()];
    //     }

    //     var manager = await _ctx.Set<BranchManager>()
    //         .FindAsync(restaurantId, branchId, user.Id);

    //     if (manager is null)
    //     {
    //         return null;
    //     }

    //     return manager.GetPermissions();
    // }

    // public async Task<Permission[]?> GetPermissions(StaffUser user)
    // {
    //     return user.GetPermissions();
    // }

    public async Task<bool> IsRestaurantOwner(
        Guid? restaurantId,
        string? masterUserId
    ) {
        return await _ctx.Set<Restaurant>()
            .AnyAsync(r =>
                r.Id == restaurantId &&
                r.OwnerId == masterUserId);
    }

    public async Task<bool> IsRestaurantManager(
        Guid? restaurantId,
        string? masterUserId
    ) {
        return await _ctx.Set<RestaurantManager>()
            .AnyAsync(rm =>
                rm.RestaurantId == restaurantId &&
                rm.MasterId == masterUserId);
    }

    public async Task<bool> IsBranchManager(
        Guid? restaurantId,
        short? branchId,
        string? masterUserId
    ) {
        return await _ctx.Set<BranchManager>()
            .AnyAsync(bm =>
                bm.RestaurantId == restaurantId &&
                bm.BranchId == branchId &&
                bm.MasterId == masterUserId);
    }

    public async Task<bool> CheckRestaurantManagerPermission(
        Guid? restaurantId,
        string? masterUserId,
        params int[] permissionIds
    ) {
        var count = await _ctx.Set<RestaurantManager>()
            .Where(rm =>
                rm.RestaurantId == restaurantId &&
                rm.MasterId == masterUserId)
            .SelectMany(rm => rm.Roles)
            .Select(rmr => rmr.Role) // extra join via navigation property
            .SelectMany(r => r.Permissions)
            .Where(rp => permissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .Distinct()
            .CountAsync();

        return count == permissionIds.Length;
    }

    public async Task<bool> CheckBranchManagerPermission(
        Guid? restaurantId,
        short? branchId,
        string? masterUserId,
        params int[] permissionIds
    ) {
        var count = await _ctx.Set<BranchManager>()
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

    public async Task<bool> CheckStaffPermission(
        Guid? restaurantId,
        short? branchId,
        short? staffId,
        params int[] permissionIds
    ) {
        var count = await _ctx.Set<StaffUser>()
            .Where(s =>
                s.RestaurantId == restaurantId &&
                s.BranchId == branchId &&
                s.Id == staffId)
            .SelectMany(s => s.Roles)
            .Select(sr => sr.Role) // extra join via navigation property
            .SelectMany(r => r.Permissions)
            .Where(rp => permissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .Distinct()
            .CountAsync();

        return count == permissionIds.Length;
    }

    // public async Task<bool> CheckPermissions(Branch branch, MasterUser user, Permission[]? permissions = null)
    // {
    //     return await _ctx.Set<Restaurant>()
    //         .AnyAsync(r =>
    //             r.Id == branch.RestaurantId && (
    //                 r.OwnerId == user.Id ||
    //                 _ctx.Set<BranchManager>().Any(m =>
    //                     m.RestaurantId == r.Id &&
    //                     m.MasterId == user.Id &&
    //                     m.BranchId == branch.Id
    //         )));
    // }

    // public async Task<bool> CheckPermissions(Branch branch, StaffUser user, Permission[]? permissions = null)
    // {
    //     return branch.RestaurantId == user.RestaurantId &&
    //            branch.Id == user.BranchId;
    // }
}