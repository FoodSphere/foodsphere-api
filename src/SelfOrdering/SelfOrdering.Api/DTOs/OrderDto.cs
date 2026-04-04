namespace FoodSphere.SelfOrdering.Api.DTO;

public record OrderRequest
{
    public ICollection<OrderItemRequest> items { get; set; } = [];

    public OrderStatus status { get; set; } = OrderStatus.Draft;
}

public record OrderStatusRequest
{
    public OrderStatus status { get; set; }
}

public record OrderResponse
{
    public required short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }

    public ICollection<OrderItemResponse> items { get; set; } = [];

    public OrderStatus status { get; set; }

    public static readonly Expression<Func<Order, OrderResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            items = model.Items
                .Select(e => OrderItemResponse.Projection.Invoke(e))
                .ToArray(),
            status = model.Status,
        };

    public static readonly Func<Order, OrderResponse> Project = Projection.Compile();
}

public record OrderItemRequest
{
    public required short menu_id { get; set; }
    public required short quantity { get; set; }

    /// <example>no spicy</example>
    public string? note { get; set; }
}

public record OrderItemUpdateRequest
{
    public required short quantity { get; set; }

    /// <example>no spicy</example>
    public string? note { get; set; }
}

public record OrderItemResponse
{
    public required short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }

    public required short menu_id { get; set; }

    public required int price_snapshot { get; set; }
    public required short quantity { get; set; }

    /// <example>no spicy</example>
    public string? note { get; set; }

    public static readonly Expression<Func<OrderItem, OrderItemResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            menu_id = model.MenuId,
            price_snapshot = model.PriceSnapshot,
            quantity = model.Quantity,
            note = model.Note,
        };

    public static readonly Func<OrderItem, OrderItemResponse> Project = Projection.Compile();
}

public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
{
    public OrderItemRequestValidator()
    {
        RuleFor(x => x.quantity)
            .GreaterThan((short)0);
    }
}

public class OrderItemUpdateRequestValidator : AbstractValidator<OrderItemUpdateRequest>
{
    public OrderItemUpdateRequestValidator()
    {
        RuleFor(x => x.quantity)
            .GreaterThan((short)0);
    }
}