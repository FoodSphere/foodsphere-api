using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Consumer.Api.Service;

public class ProfileService(
    ILogger<ProfileService> logger,
    FoodSphereDbContext context
) : ServiceBase(context)
{

}