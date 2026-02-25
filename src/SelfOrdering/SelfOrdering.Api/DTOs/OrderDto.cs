namespace FoodSphere.SelfOrdering.Api.DTO;

public class OrderDto
{
    public List<OrderItemDto> items { get; set; } = [];
}

public class OrderResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public List<OrderItemDto> items { get; set; } = [];

    public OrderStatus status { get; set; }

    public static OrderResponse FromModel(Order model)
    {
        return new OrderResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            items = [.. model.Items.Select(OrderItemDto.FromModel)],
            status = model.Status,
        };
    }
}

public class OrderItemDto
{
    public short menu_id { get; set; }
    public short quantity { get; set; }
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

public class OrderStatusDto
{
    public OrderStatus status { get; set; }
}