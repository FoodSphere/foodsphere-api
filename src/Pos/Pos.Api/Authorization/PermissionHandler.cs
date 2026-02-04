using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Pos.Api.Authorizations;

// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased
// custom requirement is better than OperationAuthorizationRequirement
// if handler making separate database calls for each requirements
// in IAuthorizationService.AuthorizeAsync()
public record PermissionRequirement(params Permission[] Permissions) : IAuthorizationRequirement;

// should I make class/record of keys for every EF Core entity?
public record RestaurantKeys(Guid RestaurantId);
public record BranchKeys(Guid RestaurantId, short BranchId);

public class MasterPermissionHandler(
    ILogger<MasterPermissionHandler> logger,
    FoodSphereDbContext dbContext
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
        var isOwner = await dbContext.Set<Restaurant>()
            .AnyAsync(r => r.Id == resource.RestaurantId && r.OwnerId == userId);

        if (isOwner)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}

public class ManagerPermissionHandler(
    ILogger<ManagerPermissionHandler> logger,
    FoodSphereDbContext dbContext
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

        if (userType is not UserType.Master)
        {
            context.Fail();
            return;
        }

        var userId = context.User.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType);
        var isOwner = await dbContext.Set<Branch>()
            .Include(b => b.Restaurant)
            .Where(b => b.Id == resource.BranchId && b.RestaurantId == resource.RestaurantId)
            .AnyAsync(b => b.Restaurant.OwnerId == userId);

        if (isOwner)
        {
            context.Succeed(requirement);
            return;
        }

        var requiredPermissionIds = requirement.Permissions.Select(p => p.Id).Distinct().ToList();

        if (requiredPermissionIds.Count == 0)
        {
            var isManager = await dbContext.Set<Manager>()
                .AnyAsync(m =>
                    m.MasterId == userId &&
                    m.RestaurantId == resource.RestaurantId &&
                    m.BranchId == resource.BranchId);

            if (isManager)
            {
                context.Succeed(requirement);
                return;
            }
        }
        else
        {
            var count = await dbContext.Set<Manager>()
                .Where(m =>
                    m.MasterId == userId &&
                    m.RestaurantId == resource.RestaurantId &&
                    m.BranchId == resource.BranchId)
                .SelectMany(m => m.Roles)
                .Select(mr => mr.Role) // extra join via navigation property
                .SelectMany(r => r.Permissions)
                .Where(rp => requiredPermissionIds.Contains(rp.PermissionId))
                .Select(rp => rp.PermissionId)
                .Distinct()
                .CountAsync();

            if (count == requiredPermissionIds.Count)
            {
                context.Succeed(requirement);
                return;
            }
        }


        context.Fail();
    }
}

public class StaffPermissionHandler(
    ILogger<StaffPermissionHandler> logger,
    FoodSphereDbContext dbContext
) : AuthorizationHandler<PermissionRequirement, BranchKeys>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement,
        BranchKeys resource)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var userTypeClaim = context.User.FindFirstValue(FoodSphereClaimType.UserTypeClaimType);
        if (!Enum.TryParse<UserType>(userTypeClaim, out var userType))
        {
            return;
        }

        if (userType is not UserType.Staff)
        {
            context.Fail();
            return;
        }

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

        var requiredPermissionIds = requirement.Permissions.Select(p => p.Id).Distinct().ToList();
        var count = await dbContext.Set<StaffUser>()
            .Where(s =>
                s.RestaurantId == restaurantId &&
                s.BranchId == branchId &&
                s.Id == staffId)
            .SelectMany(s => s.Roles)
            .Select(sr => sr.Role) // extra join via navigation property
            .SelectMany(r => r.Permissions)
            .Where(rp => requiredPermissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .Distinct()
            .CountAsync();

        if (count == requiredPermissionIds.Count)
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