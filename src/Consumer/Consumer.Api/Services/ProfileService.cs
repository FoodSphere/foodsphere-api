using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Consumer.Api.Services;

public class ProfileService(
    ILogger<ProfileService> logger,
    FoodSphereDbContext context
) : ServiceBase(context)
{

}