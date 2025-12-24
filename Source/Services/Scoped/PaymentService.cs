using Microsoft.EntityFrameworkCore;
using FoodSphere.Data.Models;

namespace FoodSphere.Services;

public class PaymentService(AppDbContext context) : BaseService(context)
{
}