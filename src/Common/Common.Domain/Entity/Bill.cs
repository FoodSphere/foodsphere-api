namespace FoodSphere.Common.Entity;

public class Bill : EntityBase
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

public class BillMember : EntityBase<short>
{
    public Guid BillId { get; set; }
    public virtual Bill Bill { get; set; } = null!;

    public Guid? ConsumerId { get; set; }
    public virtual ConsumerUser? Consumer { get; set; }

    public virtual List<Order> Orders { get; } = [];

    public string? Name { get; set; }
}

public class Order : EntityBase<short>
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

// OrderLineItem
public class OrderItem : TrackableEntityBase
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