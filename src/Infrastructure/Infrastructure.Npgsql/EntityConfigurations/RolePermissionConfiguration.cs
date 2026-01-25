namespace FoodSphere.Infrastructure.Npgsql.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.Id });

        // builder.HasIndex(r => new { r.RestaurantId, r.Name }).IsUnique();

        // builder.HasMany(r => r.Permissions)
        //        .WithMany()
        //        .UsingEntity<Dictionary<string, object>>(
        //            "RolePermission",
        //            rp => rp.HasOne<Permission>().WithMany().HasForeignKey("PermissionId").OnDelete(DeleteBehavior.Cascade),
        //            rp => rp.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade),
        //            rp =>
        //            {
        //                rp.HasKey("RoleId", "PermissionId");
        //                rp.ToTable("RolePermissions");
        //            });
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.RoleId, e.PermissionId });

        builder.HasOne(e => e.Role)
            .WithMany(e => e.Permissions)
            .HasForeignKey(e => new { e.RestaurantId, e.RoleId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(e => e.Id);
        // builder.HasIndex(p => p.Name).IsUnique();
    }
}

// public static class ModelBuilderExtensions
// {
//     public static void PermissionSeeding(this ModelBuilder modelBuilder)
//     {
//         var ViewDashboardPermission = new Permission
//         {
//             Name = "ViewDashboard",
//             Description = "view dashboard"
//         };

//         var ViewOrdersPermission = new Permission
//         {
//             Name = "ViewOrders",
//             Description = "view orders"
//         };

//         var EditOrdersPermission = new Permission
//         {
//             Name = "EditOrders",
//             Description = "edit orders"
//         };

//         var ViewTablesPermission = new Permission
//         {
//             Name = "ViewTables",
//             Description = "view tables"
//         };

//         var EditTablesPermission = new Permission
//         {
//             Name = "EditTables",
//             Description = "edit tables"
//         };

//         var ViewStocksPermission = new Permission
//         {
//             Name = "ViewStocks",
//             Description = "view stocks"
//         };

//         var EditStocksPermission = new Permission
//         {
//             Name = "EditStocks",
//             Description = "edit stocks"
//         };

//         var ViewMenusPermission = new Permission
//         {
//             Name = "ViewMenus",
//             Description = "view menus"
//         };

//         var EditMenusPermission = new Permission
//         {
//             Name = "EditMenus",
//             Description = "edit menus"
//         };

//         var ViewRestaurantPermission = new Permission
//         {
//             Name = "ViewRestaurant",
//             Description = "view restaurant"
//         };

//         var EditRestaurantPermission = new Permission
//         {
//             Name = "EditRestaurant",
//             Description = "edit restaurant"
//         };

//         var DashboardRole = new Role
//         {
//             Name = "Dashboard",
//             Description = "dashboard access",
//         };

//         DashboardRole.Permissions.Add(ViewDashboardPermission);
//         DashboardRole.Permissions.Add(ViewOrdersPermission);

//         var OrderRole = new Role
//         {
//             Name = "Order",
//             Description = "dashboard access"
//         };

//         OrderRole.Permissions.Add(ViewOrdersPermission);
//         OrderRole.Permissions.Add(EditOrdersPermission);

//         var TableRole = new Role
//         {
//             Name = "Table",
//             Description = "dashboard access"
//         };

//         TableRole.Permissions.Add(ViewTablesPermission);
//         TableRole.Permissions.Add(EditTablesPermission);

//         var StockRole = new Role
//         {
//             Name = "Stock",
//             Description = "dashboard access"
//         };

//         StockRole.Permissions.Add(ViewStocksPermission);
//         StockRole.Permissions.Add(EditStocksPermission);

//         var MenuRole = new Role
//         {
//             Name = "Menu",
//             Description = "dashboard access"
//         };

//         MenuRole.Permissions.Add(ViewMenusPermission);
//         MenuRole.Permissions.Add(EditMenusPermission);

//         var RestaurantRole = new Role
//         {
//             Name = "Restaurant",
//             Description = "dashboard access"
//         };

//         RestaurantRole.Permissions.Add(ViewRestaurantPermission);
//         RestaurantRole.Permissions.Add(EditRestaurantPermission);

//         modelBuilder.Entity<Permission>().HasData(
//             ViewDashboardPermission,

//             ViewOrdersPermission,
//             EditOrdersPermission,

//             ViewTablesPermission,
//             EditTablesPermission,

//             ViewStocksPermission,
//             EditStocksPermission,

//             ViewMenusPermission,
//             EditMenusPermission,

//             ViewRestaurantPermission,
//             EditRestaurantPermission
//         );

//         modelBuilder.Entity<Role>().HasData(
//             DashboardRole,
//             OrderRole,
//             TableRole,
//             StockRole,
//             MenuRole,
//             RestaurantRole
//         );
//     }
// }