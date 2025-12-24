using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    [PrimaryKey(nameof(RestaurantId), nameof(Id))]
    public class Branch : BaseModel<short>
    {
        public Guid RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; } = null!;

        public Guid? ContactId { get; set; }
        public virtual Contact? Contact { get; set; } = null!;

        public virtual List<Manager> BranchManagers { get; } = [];
        public virtual List<Stock> IngredientStocks { get; } = [];
        public virtual List<Table> Tables { get; } = [];
        public virtual List<StaffUser> Staffs { get; } = [];
        public virtual List<Queuing> Queuings { get; } = [];

        public required string Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Address { get; set; }
        public DateTime? OpeningTime { get; set; }
        public DateTime? ClosingTime { get; set; }
    }

    [PrimaryKey(nameof(RestaurantId), nameof(BranchId), nameof(MasterId))]
    public class Manager : TrackableModel
    {
        public Guid RestaurantId { get; set; }
        public short BranchId { get; set; }
        public virtual Branch Branch { get; set; } = null!;

        public string MasterId { get; set; } = null!;
        public virtual MasterUser Master { get; set; } = null!;

        public virtual List<ManagerRole> Roles { get; } = [];
    }

    [PrimaryKey(nameof(RestaurantId), nameof(BranchId), nameof(ManagerId), nameof(RoleId))]
    public class ManagerRole : TrackableModel
    {
        public Guid RestaurantId { get; set; }
        public short BranchId { get; set; }

        public string ManagerId { get; set; } = null!;
        public virtual Manager ManagerBranch { get; set; } = null!;

        public short RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;
    }

    namespace Configurations
    {
        public class BranchConfiguration : IEntityTypeConfiguration<Branch>
        {
            public void Configure(EntityTypeBuilder<Branch> builder)
            {
            }
        }

        public class ManagerConfiguration : IEntityTypeConfiguration<Manager>
        {
            public void Configure(EntityTypeBuilder<Manager> builder)
            {
                builder.HasOne(model => model.Branch)
                    .WithMany(branch => branch.BranchManagers)
                    .HasForeignKey(model => new { model.RestaurantId, model.BranchId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(model => model.Master)
                    .WithMany(user => user.ManagedBranches)
                    .HasForeignKey(model => model.MasterId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ManagerRoleConfiguration : IEntityTypeConfiguration<ManagerRole>
        {
            public void Configure(EntityTypeBuilder<ManagerRole> builder)
            {
                builder.HasOne(model => model.ManagerBranch)
                    .WithMany(managerBranch => managerBranch.Roles)
                    .HasForeignKey(model => new { model.RestaurantId, model.BranchId, model.ManagerId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(model => model.Role)
                    .WithMany()
                    .HasForeignKey(model => new { model.RestaurantId, model.RoleId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}