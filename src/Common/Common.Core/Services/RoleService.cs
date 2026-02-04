using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Services;

public class RoleService(
    FoodSphereDbContext dbContext
) : ServiceBase(dbContext)
{

}