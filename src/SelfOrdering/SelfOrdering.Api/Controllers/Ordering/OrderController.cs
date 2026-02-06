namespace FoodSphere.SelfOrdering.Api.Controller;

[Route("orders")]
public class OrderingController(
    ILogger<OrderingController> logger,
    BillService billService,
    MenuService menuService
) : SelfOrderingControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> ListOrders()
    {
        var orders = await billService.ListOrders(Member.BillId);

        return orders
            .Select(OrderResponse.FromModel)
            .ToList();
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(OrderDto body)
    {
        var bill = await billService.GetBill(Member.BillId);

        if (bill is null)
        {
            return NotFound();
        }

        var order = await billService.CreateOrderAsync(bill);

        foreach (var item in body.items)
        {
            var menu = await menuService.GetMenu(bill.RestaurantId, item.menu_id);

            if (menu is null)
            {
                return NotFound();
            }

            await billService.SetOrderItemAsync(order, menu, item.quantity);
        }

        await billService.SaveAsync();

        return CreatedAtAction(
            nameof(GetOrder),
            new { order_id = order.Id },
            OrderResponse.FromModel(order)
        );
    }

    [HttpGet("{order_id}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(short order_id)
    {
        var order = await billService.GetOrder(Member.BillId, order_id);

        if (order is null)
        {
            return NotFound();
        }

        return OrderResponse.FromModel(order);
    }

    [HttpDelete("{order_id}")]
    public async Task<ActionResult> CancelOrder(short order_id)
    {
        var order = await billService.GetOrder(Member.BillId, order_id);

        if (order is null)
        {
            return NotFound();
        }

        // check if order can be canceled ...

        await billService.UpdateOrderStatus(order, OrderStatus.Canceled);
        await billService.SaveAsync();

        return NoContent();
    }

    [HttpPost("{order_id}/items")]
    public async Task<ActionResult> SetOrderItem(short order_id, OrderItemDto body)
    {
        var order = await billService.GetOrder(Member.BillId, order_id);

        if (order is null)
        {
            return NotFound();
        }

        var menu = await menuService.GetMenu(order.Bill.RestaurantId, body.menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        await billService.SetOrderItemAsync(order, menu, body.quantity);
        await billService.SaveAsync();

        return NoContent();
    }
}