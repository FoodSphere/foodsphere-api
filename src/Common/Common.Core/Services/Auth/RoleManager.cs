
namespace FoodSphere.Common.Service;

// var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

// foreach (var roleName in RoleType.GetAll())
// {
//     if (!await roleManager.RoleExistsAsync(roleName))
//     {
//         await roleManager.CreateAsync(new IdentityRole(roleName));
//     }
// }

// var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

// foreach (var roleName in RoleType.GetAll())
// {
//     if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
//     {
//         roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
//     }
// }

// IServiceCollection.AddIdentityCore() still add IdentityRole even though I didn't call AddRoles()?
// IdentityBuilder.AddRoles<IdentityRole>() // services.TryAddScoped<RoleManager<TRole>>();