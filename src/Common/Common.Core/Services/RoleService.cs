namespace FoodSphere.Common.Service;

public class RoleService(
    FoodSphereDbContext dbContext
) : ServiceBase(dbContext)
{
    public async Task<Role> CreateRole(
        Guid restaurantId,
        string name,
        string? description = null,
        CancellationToken ct = default
    ) {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Role>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.RestaurantId == restaurantId);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Role>().Local
                .Where(e => e.RestaurantId == restaurantId)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Role>()
                .Where(e => e.RestaurantId == restaurantId)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var role = new Role()
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantId,
            Name = name,
            Description = description,
        };

        _ctx.Add(role);

        return role;
    }

    public Role GetRoleStub(Guid restaurantId, short roleId)
    {
        var role = new Role
        {
            RestaurantId = restaurantId,
            Id = roleId,
            Name = default!,
        };

        _ctx.Attach(role);

        return role;
    }

    public IQueryable<Role> QueryRoles()
    {
        return _ctx.Set<Role>()
            .AsExpandable();
    }

    public IQueryable<Role> QuerySingleRole(Guid restaurantId, short roleId)
    {
        return QueryRoles()
            .Where(e => e.RestaurantId == restaurantId && e.Id == roleId);
    }

    public async Task<TDto?> GetRole<TDto>(Guid restaurantId, short roleId, Expression<Func<Role, TDto>> projection)
    {
        return await QuerySingleRole(restaurantId, roleId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<Role?> GetRole(Guid restaurantId, short roleId)
    {
        var existed = await _ctx.Set<Role>()
            .AnyAsync(e =>
                e.RestaurantId == restaurantId &&
                e.Id == roleId);

        if (!existed)
        {
            return null;
        }

        return GetRoleStub(restaurantId, roleId);
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

    public async Task SetPermissions(
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
        await _ctx.AddRangeAsync(newEntities, ct);
    }

    public async Task<bool> DeleteRole(Guid restaurantId, short roleId)
    {
        var role = await _ctx.FindAsync<Role>(restaurantId, roleId);

        if (role is null)
        {
            return false;
        }

        _ctx.Remove(role);

        return true;
    }
}