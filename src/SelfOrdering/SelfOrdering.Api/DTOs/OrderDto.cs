namespace FoodSphere.SelfOrdering.Api.DTO;

public class OrderRequest
{
    public IReadOnlyCollection<OrderItemRequest> items { get; set; } = [];
}

public class OrderStatusRequest
{
    public OrderStatus status { get; set; }
}

public class OrderResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public IReadOnlyCollection<OrderItemResponse> items { get; set; } = [];

    public OrderStatus status { get; set; }

    public static readonly Func<Order, OrderResponse> Project = Projection.Compile();

    public static Expression<Func<Order, OrderResponse>> Projection =>
        model => new OrderResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
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
            menu_id = model.MenuId,
            price_snapshot = model.PriceSnapshot,
            quantity = model.Quantity,
            note = model.Note,
        };
}