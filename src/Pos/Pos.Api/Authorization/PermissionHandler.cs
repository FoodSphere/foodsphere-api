using System.Security.Claims;

namespace FoodSphere.Pos.Api.Authorization;

// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased
// custom requirement is better than OperationAuthorizationRequirement
// if handler making separate database calls for each requirements
// in IAuthorizationService.AuthorizeAsync()
public record PermissionRequirement(params Permission[] Permissions) : IAuthorizationRequirement;

// should I make class/record of keys for every EF Core entity?
public record RestaurantKeys(Guid RestaurantId);
public record BranchKeys(Guid RestaurantId, short BranchId);

public class RestaurantPermissionHandler(
    ILogger<RestaurantPermissionHandler> logger,
    AuthorizeService authorizeService
) : AuthorizationHandler<PermissionRequirement, RestaurantKeys>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement,
        RestaurantKeys resource
    ) {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var userTypeClaim = context.User.FindFirstValue(FoodSphereClaimType.UserTypeClaimType);
        if (!Enum.TryParse<UserType>(userTypeClaim, out var userType))
        {
            return;
        }

        if (userType is not UserType.Master)
        {
            context.Fail();
            return;
        }

        var userId = context.User.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType);

        if (await authorizeService.IsRestaurantOwner(resource.RestaurantId, userId))
        {
            context.Succeed(requirement);
            return;
        }

        var requiredPermissionIds = requirement.Permissions.Select(p => p.Id).Distinct().ToArray();

        if (requiredPermissionIds.Length == 0)
        {
            if (await authorizeService.IsRestaurantManager(resource.RestaurantId, userId))
            {
                context.Succeed(requirement);
                return;
            }
        }
        else
        {
            var valid = await authorizeService.CheckRestaurantManagerPermission(
                resource.RestaurantId, userId,
                requiredPermissionIds);

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
    AuthorizeService authorizeService
) : AuthorizationHandler<PermissionRequirement, BranchKeys>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement,
        BranchKeys resource
    ) {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var userTypeClaim = context.User.FindFirstValue(FoodSphereClaimType.UserTypeClaimType);
        if (!Enum.TryParse<UserType>(userTypeClaim, out var userType))
        {
            return;
        }

        switch (userType)
        {
            case UserType.Master:
                await HandleMaster(context, requirement, resource);
                return;
            case UserType.Staff:
                await HandleStaff(context, requirement, resource);
                return;
        }

        context.Fail();
    }

    async Task HandleMaster(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement,
        BranchKeys resource
    ) {
        var userId = context.User.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType);

        if (await authorizeService.IsRestaurantOwner(resource.RestaurantId, userId))
        {
            context.Succeed(requirement);
            return;
        }

        var requiredPermissionIds = requirement.Permissions.Select(p => p.Id).Distinct().ToArray();

        if (requiredPermissionIds.Length == 0)
        {
            if (await authorizeService.IsRestaurantManager(resource.RestaurantId, userId))
            {
                context.Succeed(requirement);
                return;
            }

            if (await authorizeService.IsBranchManager(resource.RestaurantId, resource.BranchId, userId))
            {
                context.Succeed(requirement);
                return;
            }
        }
        else
        {
            if (await authorizeService.CheckRestaurantManagerPermission(
                resource.RestaurantId, userId,
                requiredPermissionIds))
            {
                context.Succeed(requirement);
                return;
            }

            if (await authorizeService.CheckBranchManagerPermission(
                resource.RestaurantId, resource.BranchId, userId,
                requiredPermissionIds))
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }

    async Task HandleStaff(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement,
        BranchKeys resource
    ) {
        var _restaurantId = context.User.FindFirstValue(FoodSphereClaimType.RestaurantClaimType);
        var _branchId = context.User.FindFirstValue(FoodSphereClaimType.BranchClaimType);
        var _userId = context.User.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType);

        if (!(Guid.TryParse(_restaurantId, out var restaurantId)
            && short.TryParse(_branchId, out var branchId)
            && short.TryParse(_userId, out var staffId)))
        {
            return;
        }

        if (restaurantId != resource.RestaurantId || branchId != resource.BranchId)
        {
            context.Fail();
            return;
        }

        var requiredPermissionIds = requirement.Permissions.Select(p => p.Id).Distinct().ToArray();

        if (await authorizeService.CheckStaffPermission(
            resource.RestaurantId, resource.BranchId, staffId,
            requiredPermissionIds))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}

// public static class PermissionExtension
// {
//     extension(MasterUser user)
//     {
//         public bool HasPermissionOn(Branch branch, params PermissionType[] permissions)
//         {
//             throw new NotImplementedException();
//         }
//     }

//     extension(StaffUser user)
//     {
//         public bool HasPermissionOn(params PermissionType[] permissions)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }