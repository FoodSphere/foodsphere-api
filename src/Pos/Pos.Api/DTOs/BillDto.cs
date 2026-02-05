namespace FoodSphere.Pos.Api.DTOs;

public class BillRequest
{
    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }
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

    public Guid? consumer_id { get; set; }
    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }
    public short table_id { get; set; }

    public List<OrderResponse> orders { get; set; } = [];

    /// <example>2</example>
    public short? pax { get; set; }

    public BillStatus status { get; set; }

    public static BillResponse FromModel(Bill model)
    {
        return new BillResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            consumer_id = model.ConsumerId,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            table_id = model.TableId,
            orders = [.. model.Orders.Select(OrderResponse.FromModel)],
            pax = model.Pax,
            status = model.Status,
        };
    }
}

public class OrderDto
{
    public List<OrderItemDto> items { get; set; } = [];
}

public class OrderResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid bill_id { get; set; }

    public List<OrderItemDto> items { get; set; } = [];

    public OrderStatus status { get; set; }

    public static OrderResponse FromModel(Order model)
    {
        return new OrderResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            bill_id = model.BillId,
            items = [.. model.Items.Select(OrderItemDto.FromModel)],
            status = model.Status,
        };
    }
}

public class OrderItemDto
{
    public short menu_id { get; set; }
    public short quantity { get; set; }

    /// <example>no spicy</example>
    public string? note { get; set; }

    public static OrderItemDto FromModel(OrderItem model)
    {
        return new OrderItemDto
        {
            menu_id = model.MenuId,
            quantity = model.Quantity,
            note = model.Note,
        };
    }
}

public class OrderStatusDTO
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

    public static OrderingPortalResponse FromModel(SelfOrderingPortal model)
    {
        return new OrderingPortalResponse
        {
            id = model.Id,
            bill_id = model.BillId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            max_usage = model.MaxUsage,
            usage_count = model.UsageCount,
            valid_duration = model.ValidDuration
        };
    }
}