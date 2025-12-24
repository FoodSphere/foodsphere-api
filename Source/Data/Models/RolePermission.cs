using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    [PrimaryKey(nameof(RestaurantId), nameof(Id))]
    public class Role : BaseModel<short>
    {
        public Guid RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; } = null!;

        public required string Name { get; set; }
        public string? Description { get; set; }

        public virtual List<RolePermission> Permissions { get; } = [];
    }

    [PrimaryKey(nameof(RestaurantId), nameof(RoleId), nameof(PermissionId))]
    public class RolePermission : TrackableModel
    {
        public Guid RestaurantId { get; set; }
        public short RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;

        public int PermissionId { get; set; }
        public virtual Permission Permission { get; set; } = null!;
    }

    public class Permission : BaseModel<int>
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    namespace Configurations
    {
        public class RoleConfiguration : IEntityTypeConfiguration<Role>
        {
            public void Configure(EntityTypeBuilder<Role> builder)
            {
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
                builder.HasOne(model => model.Role)
                    .WithMany(role => role.Permissions)
                    .HasForeignKey(rp => new { rp.RestaurantId, rp.RoleId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
        {
            public void Configure(EntityTypeBuilder<Permission> builder)
            {
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
    }
}