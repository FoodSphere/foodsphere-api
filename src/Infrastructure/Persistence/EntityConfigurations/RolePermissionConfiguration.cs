namespace FoodSphere.Infrastructure.Persistence.Configuration;

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