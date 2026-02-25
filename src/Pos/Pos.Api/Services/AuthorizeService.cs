using System.Security.Claims;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Pos.Api.Service;

public class AccessControlService(
    FoodSphereDbContext context,
    IAuthorizationService authorizationService
) : ServiceBase(context)
{
    public async Task<bool> Validate(
        ClaimsPrincipal user,
        Guid restaurantId,
        params Permission[] permissions
    ) {
        var result = await authorizationService.AuthorizeAsync(user,
            new RestaurantKeys(restaurantId),
            new PermissionRequirement(permissions));

        return result.Succeeded;
    }

    public async Task<bool> Validate(
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

    public async Task<bool> Validate(HttpContext httpContext, params Permission[] permissions)
    {
        var restaurant_id = httpContext.GetRouteValue("restaurant_id") as string;
        var branch_id = httpContext.GetRouteValue("branch_id") as string;
        var bill_id = httpContext.GetRouteValue("bill_id") as string;
        object resource;

        if (bill_id is not null)
        {
            Guid.TryParse(bill_id, out var billId);
            resource = new BillKeys(billId);
        }
        else if (restaurant_id is not null)
        {
            Guid.TryParse(restaurant_id, out var restaurantId);

            if (branch_id is not null)
            {
                short.TryParse(branch_id, out var branchId);
                resource = new BranchKeys(restaurantId, branchId);
            }
            else
            {
                resource = new RestaurantKeys(restaurantId);
            }
        }
        else
        {
            return true;
        }

        var result = await authorizationService.AuthorizeAsync(
            httpContext.User, resource, new PermissionRequirement(permissions));

        return result.Succeeded;
    }
}