using System.Security.Claims;
using FoodSphere.Infrastructure.Repository;

namespace FoodSphere.Pos.Api.Service;

public class AccessControlService(
    IAuthorizationService authorizationService,
    AuthorizeService authorizeService,
    BillRepository billRepository
) : ServiceBase
{
    public async Task<bool> Validate(ClaimsPrincipal user, IEntityKey key,
        params Permission[] permissions)
    {
        var result = await authorizationService.AuthorizeAsync(
            user, key, new PermissionRequirement(permissions));

        return result.Succeeded;
    }

    public async Task<bool> Validate(HttpContext httpContext,
        params Permission[] permissions)
    {
        var restaurant_id = httpContext.GetRouteValue("restaurant_id") as string;
        var branch_id = httpContext.GetRouteValue("branch_id") as string;
        var bill_id = httpContext.GetRouteValue("bill_id") as string;
        object resource;

        if (bill_id is not null)
        {
            if (!Guid.TryParse(bill_id, out var billId))
                return false;

            resource = new BillKey(billId);
        }
        else if (restaurant_id is not null)
        {
            if (!Guid.TryParse(restaurant_id, out var restaurantId))
                return false;

            if (branch_id is not null)
            {
                if (!short.TryParse(branch_id, out var branchId))
                    return false;

                resource = new BranchKey(restaurantId, branchId);
            }
            else
            {
                resource = new RestaurantKey(restaurantId);
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

    public async Task<ResultObject> Authorize(
        HttpContext context,
        params Permission[] permissions)
    {
        Guid restaurantId = default;

        if (context.GetRouteValue("restaurant_id") is string restaurantRoute)
            if (!Guid.TryParse(restaurantRoute, out restaurantId))
                return ResultObject.Fail(ResultError.Argument,
                    "invalid restaurant id");

        if (context.GetRouteValue("bill_id") is string billRoute)
        {
            if (!Guid.TryParse(billRoute, out var billId))
                return ResultObject.Fail(ResultError.Argument,
                    "invalid bill id");

            var billKey = new BillKey(billId);
            var bill = await billRepository.GetBill(billKey);

            if (bill is null)
                return ResultObject.NotFound(billKey);

            if (restaurantId != default)
            {
                if (bill.RestaurantId != restaurantId)
                    return ResultObject.Fail(ResultError.Argument,
                        "bill does not belong to the specified restaurant");
            }
            else
            {
                restaurantId = bill.RestaurantId;
            }
        }

        if (restaurantId != default)
        {
            var result = await authorizeService.GetMissingPermissions(
                context.User, new(restaurantId), permissions);

            if (!result.TryGetValue(out var missingPermissions))
                return result.Errors;

            if (missingPermissions.Length > 0)
                return ResultObject.Fail(ResultError.Forbidden,
                    "required permissions", missingPermissions);
        }

        return ResultObject.Success();
    }
}