using System.Security.Claims;

namespace FoodSphere.Pos.Api.Service;

public class AuthorizeService(
    ILogger<RestaurantPermissionHandler> logger,
    AuthorizeHelperService helperService
) : ServiceBase
{
    public async Task<ResultObject<Permission[]>> GetMissingPermissions(
        ClaimsPrincipal user, RestaurantKey restaurantKey,
        params Permission[] permissions)
    {
        if (user.Identity?.IsAuthenticated != true)
            return ResultObject.Fail(ResultError.Authentication);

        var userTypeClaim = user.FindFirstValue(FoodSphereClaimType.UserTypeClaimType);
        if (!Enum.TryParse<UserType>(userTypeClaim, out var userType))
            return ResultObject.Fail(ResultError.Authentication);

        if (userType is UserType.Master)
            return await HandleMaster(user, restaurantKey, permissions);

        else if (userType is UserType.Worker)
            return await HandleWorker(user, restaurantKey, permissions);

        return ResultObject.Fail(ResultError.Forbidden);
    }

    async Task<ResultObject<Permission[]>> HandleMaster(
        ClaimsPrincipal user, RestaurantKey restaurantKey,
        params Permission[] permissions)
    {
        var userId = user.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType)!;

        if (await helperService.IsRestaurantOwner(
            new(restaurantKey.Id), new(userId)))
        {
            return ResultObject.Success(Array.Empty<Permission>());
        }

        if (permissions.Length == 0)
        {
            if (await helperService.IsRestaurantStaff(
                new(restaurantKey.Id), new(userId)))
            {
                return ResultObject.Success(Array.Empty<Permission>());
            }
        }
        else
        {
            var missing = await helperService.GetMissingRestaurantStaffPermissions(
                new(restaurantKey.Id, userId), permissions);

            return ResultObject.Success(missing);
        }

        return ResultObject.Fail(ResultError.Forbidden);
    }

    async Task<ResultObject<Permission[]>> HandleWorker(
        ClaimsPrincipal user, RestaurantKey restaurantKey,
        params Permission[] permissions)
    {
        var _restaurantId = user.FindFirstValue(FoodSphereClaimType.RestaurantClaimType);
        var _branchId = user.FindFirstValue(FoodSphereClaimType.BranchClaimType);
        var _userId = user.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType);

        if (!(Guid.TryParse(_restaurantId, out var restaurantId)
            && short.TryParse(_branchId, out var branchId)
            && short.TryParse(_userId, out var workerId)))
            return ResultObject.Fail(ResultError.Authentication);

        if (restaurantId != restaurantKey.Id) //|| branchId != resource.Id)
            return ResultObject.Fail(ResultError.Forbidden);

        var missing = await helperService.GetMissingWorkerPermissions(
            new(restaurantId, branchId, workerId), permissions);

        return ResultObject.Success(missing);
    }
}