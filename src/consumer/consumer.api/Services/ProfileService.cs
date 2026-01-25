using FoodSphere.Infrastructure.Npgsql;

namespace FoodSphere.Consumer.Api.Services;

public class ProfileService(
    ILogger<ProfileService> logger,
    FoodSphereDbContext context
) : ServiceBase(context)
{

}