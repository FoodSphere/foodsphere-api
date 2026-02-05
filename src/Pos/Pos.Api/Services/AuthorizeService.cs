using System.Security.Claims;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Pos.Api.Services;

public class CheckPermissionService(
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
}