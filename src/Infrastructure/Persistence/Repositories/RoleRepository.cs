namespace FoodSphere.Infrastructure.Repository;

public class RoleRepository(
    FoodSphereDbContext context
) : RepositoryBase(context)
{
    public async Task<ResultObject<Role>> CreateRole(
        RestaurantKey restaurantKey,
        string name,
        string? description,
        CancellationToken ct = default)
    {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Role>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.RestaurantId == restaurantKey.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Role>().Local
                .Where(e => e.RestaurantId == restaurantKey.Id)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Role>()
                .Where(e => e.RestaurantId == restaurantKey.Id)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var role = new Role()
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantKey.Id,
            Name = name,
            Description = description,
        };

        _ctx.Add(role);

        return role;
    }

    Role CreateRoleStub(RoleKey key)
    {
        var role = new Role
        {
            RestaurantId = key.RestaurantId,
            Id = key.Id,
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

    public IQueryable<Role> QuerySingleRole(RoleKey key)
    {
        return QueryRoles()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.Id == key.Id);
    }

    public async Task<Role?> GetRole(RoleKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<Role>(key, ct);
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

    public async Task<ResultObject> SetPermissions(
        RoleKey key, IEnumerable<PermissionKey> permissionKeys,
        CancellationToken ct = default)
    {
        var desired = permissionKeys
            .Distinct()
            .ToArray();

        var existed = await _ctx.Set<RolePermission>()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.RoleId == key.Id)
            .ToArrayAsync(ct);

        var toRemove = existed
            .ExceptBy(desired.Select(e => e.Id), e => e.PermissionId)
            .ToArray();

        var toAdd = desired
            .ExceptBy(existed.Select(e => e.PermissionId), e => e.Id)
            .Select(permissionKey => new RolePermission
            {
                RestaurantId = key.RestaurantId,
                RoleId = key.Id,
                PermissionId = permissionKey.Id,
            });

        _ctx.RemoveRange(toRemove);
        _ctx.AddRange(toAdd);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteRole(
        RoleKey key, CancellationToken ct = default)
    {
        var role = await GetRole(key, ct);

        if (role is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(role);

        return ResultObject.Success();
    }
}