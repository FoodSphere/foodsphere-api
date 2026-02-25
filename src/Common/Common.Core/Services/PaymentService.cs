using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Service;

public class PaymentService(FoodSphereDbContext context) : ServiceBase(context)
{
}