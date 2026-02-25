using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Service;

public class PermissionService(
    FoodSphereDbContext dbContext
) : ServiceBase(dbContext)
{
    public async Task<List<Permission>> ListPermissions()
    {
        return await _ctx.Set<Permission>().ToListAsync();
    }
}