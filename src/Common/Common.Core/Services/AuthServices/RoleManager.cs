
namespace FoodSphere.Common.Service;

// var roleStaff = sp.GetRequiredService<RoleStaff<IdentityRole>>();

// foreach (var roleName in RoleType.GetAll())
// {
//     if (!await roleStaff.RoleExistsAsync(roleName))
//     {
//         await roleStaff.CreateAsync(new IdentityRole(roleName));
//     }
// }

// var roleStaff = sp.GetRequiredService<RoleStaff<IdentityRole>>();

// foreach (var roleName in RoleType.GetAll())
// {
//     if (!roleStaff.RoleExistsAsync(roleName).GetAwaiter().GetResult())
//     {
//         roleStaff.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
//     }
// }

// IServiceCollection.AddIdentityCore() still add IdentityRole even though I didn't call AddRoles()?
// IdentityBuilder.AddRoles<IdentityRole>() // services.TryAddScoped<RoleStaff<TRole>>();