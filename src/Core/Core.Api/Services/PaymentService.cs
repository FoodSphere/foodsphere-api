using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Npgsql;

namespace FoodSphere.Core.Services;

public class PaymentService(FoodSphereDbContext context) : ServiceBase(context)
{
}