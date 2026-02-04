using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Services;

public class RoleService(
    FoodSphereDbContext dbContext
) : ServiceBase(dbContext)
{
    public async Task<Role?> GetRole(Guid restaurantId, short roleId)
    {
        // Eager Loading
        return await _ctx.Set<Role>()
            .Include(r => r.Permissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.RestaurantId == restaurantId && r.Id == roleId);
    }

    public async Task<List<Role>> ListRoles(Guid restaurantId)
    {
        return await _ctx.Set<Role>()
            .Where(r => r.RestaurantId == restaurantId)
            .ToListAsync();
    }

    public async Task<Role> CreateRole(
        Guid restaurantId,
        string name,
        string? description = null,
        CancellationToken ct = default
    ) {
        short lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Role>()
            .Any(e => e.State == EntityState.Added && e.Entity.RestaurantId == restaurantId);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Role>().Local
                .Where(role => role.RestaurantId == restaurantId)
                .Max(role => (short?)role.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Role>()
                .Where(role => role.RestaurantId == restaurantId)
                .Select(role => (short?)role.Id)
                .MaxAsync(ct) ?? 0;
        }

        var role = new Role()
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantId,
            Name = name,
            Description = description,
        };

        await _ctx.AddAsync(role, ct);

        return role;
    }

    public async Task<bool> DeleteRole(Guid restaurantId, short roleId)
    {
        var role = await _ctx.FindAsync<Role>(restaurantId, roleId);

        if (role == null)
        {
            return false;
        }

        _ctx.Remove(role);

        return true;
    }

    // public async Task AddPermission(Role role, IEnumerable<int> permissionIds)
    // {
    //     // SELECT *
    //     if (!_ctx.Entry(role).Collection(r => r.Permissions).IsLoaded)
    //     {
    //         await _ctx.Entry(role).Collection(r => r.Permissions).LoadAsync();
    //     }

    //     var newRolePermissions = _ctx.Set<Permission>()
    //         .Select(p => p.Id)
    //         .Intersect(permissionIds)
    //         .Except(role.Permissions.Select(rp => rp.PermissionId))
    //         .Select(pid => new RolePermission
    //         {
    //             RestaurantId = role.RestaurantId,
    //             RoleId = role.Id,
    //             PermissionId = pid
    //         });

    //     role.Permissions.AddRange(newRolePermissions);
    // }

    public async Task SetPermissionsAsync(
        Guid restaurantId,
        short roleId,
        IEnumerable<int> permissionIds,
        CancellationToken ct = default
    ) {
        var desiredIds = permissionIds
            .Distinct()
            .ToArray();

        var currentRolePermissions = await _ctx.Set<RolePermission>()
            .Where(rp => rp.RestaurantId == restaurantId && rp.RoleId == roleId)
            .ToArrayAsync(ct);

        var toRemove = currentRolePermissions
            .ExceptBy(desiredIds, rp => rp.PermissionId)
            .ToArray();

        var toAddIds = desiredIds
            .Except(currentRolePermissions.Select(rp => rp.PermissionId))
            .ToArray();

        var newEntities = toAddIds.Select(pid => new RolePermission
        {
            RestaurantId = restaurantId,
            RoleId = roleId,
            PermissionId = pid
        });

        _ctx.RemoveRange(toRemove);
        await _ctx.Set<RolePermission>().AddRangeAsync(newEntities, ct);
    }
}