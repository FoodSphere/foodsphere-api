using System.Security.Claims;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Pos.Api.Services;

public class AuthorizeService(
    FoodSphereDbContext context,
    IAuthorizationService authorizationService
) : ServiceBase(context)
{
    public async Task<bool> CheckPermission(
        ClaimsPrincipal user,
        Guid restaurantId,
        params Permission[] permissions
    ) {
        var result = await authorizationService.AuthorizeAsync(user,
            new RestaurantKeys(restaurantId),
            new PermissionRequirement(permissions));

        return result.Succeeded;
    }

    public async Task<bool> CheckPermission(
        ClaimsPrincipal user,
        Guid restaurantId,
        short branchId,
        params Permission[] permissions
    ) {
        var result = await authorizationService.AuthorizeAsync(user,
            new BranchKeys(restaurantId, branchId),
            new PermissionRequirement(permissions));

        return result.Succeeded;
    }

    public async Task<Permission[]?> GetPermissions(
        MasterUser user,
        Guid restaurantId,
        short branchId
    ) {
        var restaurant = await _ctx.Set<Restaurant>()
            .FindAsync(restaurantId)
            ?? throw new InvalidOperationException();

        if (restaurant.OwnerId == user.Id)
        {
            return [.. _ctx.Set<Permission>()];
        }

        var manager = await _ctx.Set<BranchManager>()
            .FindAsync(restaurantId, branchId, user.Id);

        if (manager is null)
        {
            return null;
        }

        return manager.GetPermissions();
    }

    public async Task<Permission[]?> GetPermissions(StaffUser user)
    {
        return user.GetPermissions();
    }
}