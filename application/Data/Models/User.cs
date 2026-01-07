using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    public class MasterUser : IdentityUser
    {
        public virtual List<Restaurant> OwnedRestaurants { get; } = [];
        public virtual List<Manager> ManagedBranches { get; } = [];
    }

    [PrimaryKey(nameof(RestaurantId), nameof(BranchId), nameof(Id))]
    public class StaffUser : BaseModel<short>
    {
        public Guid RestaurantId { get; set; }
        public short BranchId { get; set; }
        public virtual Branch Branch { get; set; } = null!;

        public required string Name { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }

        public virtual List<StaffRole> Roles { get; } = [];
    }

    [PrimaryKey(nameof(RestaurantId), nameof(BranchId), nameof(StaffId), nameof(RoleId))]
    public class StaffRole : TrackableModel
    {
        public Guid RestaurantId { get; set; }
        public short BranchId { get; set; }

        public short StaffId { get; set; }
        public virtual StaffUser Staff { get; set; } = null!;

        public short RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;
    }

    public class ConsumerUser : BaseModel
    {
        public virtual List<Bill> Bills { get; } = [];

        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool TwoFactorEnabled { get; set; } = false;

        public string GetSecurityStamp()
        {
            throw new NotImplementedException();
        }
    }

    namespace Configurations
    {
        public class MasterConfiguration : IEntityTypeConfiguration<MasterUser>
        {
            public void Configure(EntityTypeBuilder<MasterUser> builder)
            {
            }
        }

        public class StaffConfiguration : IEntityTypeConfiguration<StaffUser>
        {
            public void Configure(EntityTypeBuilder<StaffUser> builder)
            {
                builder.HasOne(model => model.Branch)
                    .WithMany(branch => branch.Staffs)
                    .HasForeignKey(model => new { model.RestaurantId, model.BranchId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class StaffRoleConfiguration : IEntityTypeConfiguration<StaffRole>
        {
            public void Configure(EntityTypeBuilder<StaffRole> builder)
            {
                builder.HasOne(model => model.Staff)
                    .WithMany(staff => staff.Roles)
                    .HasForeignKey(model => new { model.RestaurantId, model.BranchId, model.StaffId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(model => model.Role)
                    .WithMany()
                    .HasForeignKey(model => new { model.RestaurantId, model.RoleId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ConsumerConfiguration : IEntityTypeConfiguration<ConsumerUser>
        {
            public void Configure(EntityTypeBuilder<ConsumerUser> builder)
            {
            }
        }
    }
}