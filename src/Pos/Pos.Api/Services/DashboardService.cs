using Microsoft.EntityFrameworkCore;
using FoodSphere.Core.Services;
using FoodSphere.Infrastructure.Npgsql;

namespace FoodSphere.Pos.Api.Services;

public class DashboardService(FoodSphereDbContext context) : ServiceBase(context)
{

}