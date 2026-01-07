using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    public class Bill : BaseModel
    {
        public Guid? ConsumerId { get; set; }
        public virtual ConsumerUser? Consumer { get; set; }

        public Guid RestaurantId { get; set; }
        public short BranchId { get; set; }
        public short? IssuerId { get; set; }

        public short TableId { get; set; }
        public virtual Table Table { get; set; } = null!;

        public virtual List<BillMember> Members { get; } = [];
        public virtual List<SelfOrderingPortal> Portals { get; } = [];
        public virtual List<Order> Orders { get; } = [];

        public short? Pax { get; set; }
        public BillStatus Status { get; set; }
    }

    [PrimaryKey(nameof(BillId), nameof(Id))]
    public class BillMember : BaseModel<short>
    {
        public Guid BillId { get; set; }
        public virtual Bill Bill { get; set; } = null!;

        public Guid? ConsumerId { get; set; }
        public virtual ConsumerUser? Consumer { get; set; }

        public virtual List<Order> Orders { get; } = [];

        public string? Name { get; set; }
    }

    [PrimaryKey(nameof(BillId), nameof(Id))]
    public class Order : BaseModel<short>
    {
        public Guid BillId { get; set; }
        public virtual Bill Bill { get; set; } = null!;

        public Guid? RestaurantId { get; set; }
        public short? BranchId { get; set; }
        public short? IssuerId { get; set; }

        public short? BillMemberId { get; set; }
        public virtual BillMember? BillMember { get; set; }

        public virtual List<OrderItem> Items { get; } = [];

        public OrderStatus Status { get; set; }
    }

    [PrimaryKey(nameof(BillId), nameof(RestaurantId), nameof(OrderId), nameof(MenuId))]
    public class OrderItem : TrackableModel
    {
        public Guid BillId { get; set; }
        public short OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

        public Guid RestaurantId { get; set; }
        public short MenuId { get; set; }
        public virtual Menu Menu { get; set; } = null!;

        public int PriceSnapshot { get; set; }
        public short Quantity { get; set; }
        public string? Note { get; set; }
        // public OrderItemStatus Status { get; set; }
    }

    namespace Configurations
    {
        public class BillConfiguration : IEntityTypeConfiguration<Bill>
        {
            public void Configure(EntityTypeBuilder<Bill> builder)
            {
                builder.HasOne(model => model.Table)
                    .WithMany()
                    .HasForeignKey(model => new { model.RestaurantId, model.BranchId, model.TableId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne<StaffUser>()
                    .WithMany()
                    .HasForeignKey(model => new { model.RestaurantId, model.BranchId, model.IssuerId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class BillMemberConfiguration : IEntityTypeConfiguration<BillMember>
        {
            public void Configure(EntityTypeBuilder<BillMember> builder)
            {
            }
        }

        public class OrderConfiguration : IEntityTypeConfiguration<Order>
        {
            public void Configure(EntityTypeBuilder<Order> builder)
            {
                builder.HasOne<StaffUser>()
                    .WithMany()
                    .HasForeignKey(model => new { model.RestaurantId, model.BranchId, model.IssuerId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(model => model.BillMember)
                    .WithMany(billMember => billMember.Orders)
                    .HasForeignKey(model => new { model.BillId, model.BillMemberId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
        {
            public void Configure(EntityTypeBuilder<OrderItem> builder)
            {
                builder.HasOne(model => model.Order)
                    .WithMany(order => order.Items)
                    .HasForeignKey(model => new { model.BillId, model.OrderId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(model => model.Menu)
                    .WithMany()
                    .HasForeignKey(model => new { model.RestaurantId, model.MenuId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}