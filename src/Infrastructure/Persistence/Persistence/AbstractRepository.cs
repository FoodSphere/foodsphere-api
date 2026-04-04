namespace FoodSphere.Infrastructure.Repository;

public abstract class RepositoryBase(FoodSphereDbContext context)
{
    protected readonly FoodSphereDbContext _ctx = context;
}