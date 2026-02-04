using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Pos.Api.Services;

public class DashboardService(FoodSphereDbContext context) : ServiceBase(context)
{

}