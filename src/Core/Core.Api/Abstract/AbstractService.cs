using FoodSphere.Infrastructure.Npgsql;

namespace FoodSphere.Core.Services;

public abstract class ServiceBase(FoodSphereDbContext context)
{
    protected readonly FoodSphereDbContext _ctx = context;

    public async Task<int> SaveAsync()
    {
        return await _ctx.SaveChangesAsync();
    }
}