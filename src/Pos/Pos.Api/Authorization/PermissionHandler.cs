using System.Security.Claims;

namespace FoodSphere.Pos.Api.Authorization;

// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased
// custom requirement is better than OperationAuthorizationRequirement
// if handler making separate database calls for each requirements
// in IAuthorizationService.AuthorizeAsync()
public record PermissionRequirement(params Permission[] Permissions) : IAuthorizationRequirement;

public class RestaurantPermissionHandler(
    ILogger<RestaurantPermissionHandler> logger,
    AuthorizeHelperService authorizeService
) : AuthorizationHandler<PermissionRequirement, RestaurantKey>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement,
        RestaurantKey resource)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        var userTypeClaim = context.User.FindFirstValue(FoodSphereClaimType.UserTypeClaimType);
        if (!Enum.TryParse<UserType>(userTypeClaim, out var userType))
            return;

        if (userType is not UserType.Master)
        {
            context.Fail();
            return;
        }

        var userId = context.User.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType)!;

        if (await authorizeService.IsRestaurantOwner(
            new(resource.Id), new(userId)))
        {
            context.Succeed(requirement);
            return;
        }

        var requiredPermissionIds = requirement.Permissions
            .Select(p => p.Id)
            .Distinct()
            .ToArray();

        if (requiredPermissionIds.Length == 0)
        {
            if (await authorizeService.IsRestaurantStaff(
                new(resource.Id), new(userId)))
            {
                context.Succeed(requirement);
                return;
            }
        }
        else
        {
            var valid = await authorizeService.CheckRestaurantStaffPermission(
                new(resource.Id, userId), requiredPermissionIds);

            if (valid)
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }
}

public class BranchPermissionHandler(
    ILogger<BranchPermissionHandler> logger,
    AuthorizeHelperService authorizeService
) : AuthorizationHandler<PermissionRequirement, BranchKey>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement,
        BranchKey resource)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        var userTypeClaim = context.User.FindFirstValue(FoodSphereClaimType.UserTypeClaimType);
        if (!Enum.TryParse<UserType>(userTypeClaim, out var userType))
            return;

        switch (userType)
        {
            case UserType.Master:
                await HandleMaster(context, requirement, resource);
                return;
            case UserType.Worker:
                await HandleWorker(context, requirement, resource);
                return;
        }

        context.Fail();
    }

    async Task HandleMaster(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement,
        BranchKey resource)
    {
        var userId = context.User.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType)!;

        if (await authorizeService.IsRestaurantOwner(
            new(resource.RestaurantId), new(userId)))
        {
            context.Succeed(requirement);
            return;
        }

        var requiredPermissionIds = requirement.Permissions.Select(p => p.Id).Distinct().ToArray();

        if (requiredPermissionIds.Length == 0)
        {
            if (await authorizeService.IsRestaurantStaff(
                new(resource.RestaurantId), new(userId)))
            {
                context.Succeed(requirement);
                return;
            }

            if (await authorizeService.IsBranchStaff(resource.RestaurantId, resource.Id, userId))
            {
                context.Succeed(requirement);
                return;
            }
        }
        else
        {
            if (await authorizeService.CheckRestaurantStaffPermission(
                new(resource.RestaurantId, userId), requiredPermissionIds))
            {
                context.Succeed(requirement);
                return;
            }

            if (await authorizeService.CheckBranchStaffPermission(
                resource.RestaurantId, resource.Id, userId,
                requiredPermissionIds))
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }

    async Task HandleWorker(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement,
        BranchKey resource)
    {
        var _restaurantId = context.User.FindFirstValue(FoodSphereClaimType.RestaurantClaimType);
        var _branchId = context.User.FindFirstValue(FoodSphereClaimType.BranchClaimType);
        var _userId = context.User.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType);

        if (!(Guid.TryParse(_restaurantId, out var restaurantId)
            && short.TryParse(_branchId, out var branchId)
            && short.TryParse(_userId, out var workerId)))
            return;

        if (restaurantId != resource.RestaurantId || branchId != resource.Id)
        {
            context.Fail();
            return;
        }

        var requiredPermissionIds = requirement.Permissions.Select(p => p.Id).Distinct().ToArray();

        if (await authorizeService.CheckWorkerPermission(
            new(resource.RestaurantId, resource.Id, workerId),
            requiredPermissionIds))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}

public class BillPermissionHandler(
    ILogger<BillPermissionHandler> logger,
    AuthorizeHelperService authorizeService
) : AuthorizationHandler<PermissionRequirement, BillKey>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement,
        BillKey resource
    )
    {
    }
}