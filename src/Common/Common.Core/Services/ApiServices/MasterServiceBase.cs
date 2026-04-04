namespace FoodSphere.Common.Service;

public class MasterServiceBase(
    FoodSphereDbContext context,
    MasterRepository masterRepository
) : ServiceBase
{
}