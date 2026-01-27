using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Services;

public class PaymentService(FoodSphereDbContext context) : ServiceBase(context)
{
}