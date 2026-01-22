using Microsoft.EntityFrameworkCore;
using FoodSphere.Data.Models;
using FoodSphere.Data.DTOs;

namespace FoodSphere.Services;

public class DashboardService(AppDbContext context) : BaseService(context)
{

}