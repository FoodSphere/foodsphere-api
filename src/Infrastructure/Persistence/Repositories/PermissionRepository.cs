namespace FoodSphere.Infrastructure.Repository;

public class PermissionRepository(
    FoodSphereDbContext dbContext
) : RepositoryBase(dbContext)
{
    public IQueryable<Permission> QueryPermissions()
    {
        return _ctx.Set<Permission>()
            .AsExpandable();
    }
}