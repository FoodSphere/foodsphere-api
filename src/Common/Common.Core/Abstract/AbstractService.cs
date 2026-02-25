using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Service;

public abstract class ServiceBase(FoodSphereDbContext context)
{
    protected readonly FoodSphereDbContext _ctx = context;

    public async Task<int> SaveChanges()
    {
        return await _ctx.SaveChangesAsync();
    }
}