namespace FoodSphere.Pos.Api.DTO;

public class BillRequest
{
    public short table_id { get; set; }

    public Guid? consumer_id { get; set; }

    /// <example>2</example>
    public short? pax { get; set; }
}

public class BillResponse
{
    public Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }
    public short table_id { get; set; }
    public Guid? consumer_id { get; set; }

    public IReadOnlyCollection<OrderResponse> orders { get; set; } = [];

    /// <example>2</example>
    public short? pax { get; set; }

    public BillStatus status { get; set; }

    public static readonly Func<Bill, BillResponse> Project = Projection.Compile();

    public static Expression<Func<Bill, BillResponse>> Projection =>
        model => new BillResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            consumer_id = model.ConsumerId,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            table_id = model.TableId,
            orders = model.Orders
                .Select(e => OrderResponse.Projection.Invoke(e))
                .ToArray(),
            pax = model.Pax,
            status = model.Status,
        };
}

public class OrderRequest
{
    public IReadOnlyCollection<OrderItemRequest> items { get; set; } = [];
}

public class OrderResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid bill_id { get; set; }

    public IReadOnlyCollection<OrderItemResponse> items { get; set; } = [];

    public OrderStatus status { get; set; }

    public static readonly Func<Order, OrderResponse> Project = Projection.Compile();

    public static Expression<Func<Order, OrderResponse>> Projection =>
        model => new OrderResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            bill_id = model.BillId,
            items = model.Items
                .Select(e => OrderItemResponse.Projection.Invoke(e))
                .ToArray(),
            status = model.Status,
        };
}

public class OrderItemRequest
{
    public short menu_id { get; set; }
    public short quantity { get; set; }

    /// <example>no spicy</example>
    public string? note { get; set; }
}

public class OrderItemUpdateRequest
{
    public short quantity { get; set; }

    /// <example>no spicy</example>
    public string? note { get; set; }
}

public class OrderItemResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid bill_id { get; set; }
    public short order_id { get; set; }

    public Guid restaurant_id { get; set; }
    public short menu_id { get; set; }

    public int price_snapshot { get; set; }
    public short quantity { get; set; }

    /// <example>no spicy</example>
    public string? note { get; set; }

    public static readonly Func<OrderItem, OrderItemResponse> Project = Projection.Compile();

    public static Expression<Func<OrderItem, OrderItemResponse>> Projection =>
        model => new OrderItemResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            bill_id = model.BillId,
            order_id = model.OrderId,
            restaurant_id = model.RestaurantId,
            menu_id = model.MenuId,
            price_snapshot = model.PriceSnapshot,
            quantity = model.Quantity,
            note = model.Note,
        };
}

public class OrderStatusRequest
{
    public OrderStatus status { get; set; }
}

public class OrderingPortalRequest
{
    /// <example>2</example>
    public short? max_usage { get; set; }

    public TimeSpan? valid_duration { get; set; }
}

public class OrderingPortalResponse
{
    public Guid id { get; set; }
    public Guid bill_id { get; set; }
    // public Guid? consumer_id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    /// <example>2</example>
    public short? max_usage { get; set; }

    /// <example>0</example>
    public short usage_count { get; set; }

    public TimeSpan? valid_duration { get; set; }

    public static readonly Func<SelfOrderingPortal, OrderingPortalResponse> Project = Projection.Compile();

    public static Expression<Func<SelfOrderingPortal, OrderingPortalResponse>> Projection =>
        model => new OrderingPortalResponse
        {
            id = model.Id,
            bill_id = model.BillId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            max_usage = model.MaxUsage,
            usage_count = model.UsageCount,
            valid_duration = model.ValidDuration,
        };
}