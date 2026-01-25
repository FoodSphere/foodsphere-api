using FoodSphere.Core.Services;
using FoodSphere.Infrastructure.Npgsql;

namespace FoodSphere.Pos.Api.Services;

public class AuthorizeService(FoodSphereDbContext context) : ServiceBase(context)
{
//     public async Task<bool> CheckPermission(ClientUser user, Restaurant restaurant, string permission)
//     {
//         user.
//         return user.Permissions.Any(p => p.Name == permission);
//     }

//     public async Task<bool> CheckPermission(StaffUser user, string permission)
//     {
//         user.
//         return user.Permissions.Any(p => p.Name == permission);
//     }
}