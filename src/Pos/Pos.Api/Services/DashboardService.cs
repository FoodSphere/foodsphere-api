using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Pos.Api.Service;

public class DashboardService(FoodSphereDbContext context) : ServiceBase(context)
{

}