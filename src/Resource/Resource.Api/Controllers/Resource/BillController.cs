namespace FoodSphere.Resource.Api.Controller;

[Route("bills")]
public class BillController(
    ILogger<BillController> logger,
    BillService billService,
    MenuService menuService,
    BranchService branchService
) : ResourceControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BillResponse>>> ListBills()
    {
        var bills = await billService.ListBills();

        return bills.Select(BillResponse.FromModel).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<BillResponse>> CreateBill(BillRequest body)
    {
        var table = await branchService.GetTable(
            restaurantId: body.restaurant_id,
            branchId: body.branch_id,
            tableId: body.table_id
        );

        if (table is null)
        {
            return NotFound();
        }

        var bill = await billService.CreateBillAsync(
            consumerId: body.consumer_id,
            table: table,
            pax: body.pax
        );

        await billService.SaveAsync();

        return CreatedAtAction(
            nameof(GetBill),
            new { bill_id = bill.Id },
            BillResponse.FromModel(bill)
        );
    }

    [HttpGet("{bill_id}")]
    public async Task<ActionResult<BillResponse>> GetBill(Guid bill_id)
    {
        var bill = await billService.GetBill(bill_id);

        if (bill is null) return NotFound();

        return BillResponse.FromModel(bill);
    }

    // [HttpPut("{bill_id}")]
    // public async Task<ActionResult> UpdateBill(Guid bill_id, BillRequest bill)
    // {
    //     return NoContent();
    // }

    [HttpDelete("{bill_id}")]
    public async Task<ActionResult> DeleteBill(Guid bill_id)
    {
        var bill = await billService.GetBill(bill_id);

        if (bill is null)
        {
            return NotFound();
        }

        await billService.DeleteBill(bill);
        await billService.SaveAsync();

        return NoContent();
    }

    [HttpPost("{bill_id}/orders")]
    public async Task<ActionResult<OrderResponse>> CreateOrder(Guid bill_id, OrderRequest body)
    {
        var bill = await billService.GetBill(bill_id);

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
            new { bill_id, order_id = order.Id },
            OrderResponse.FromModel(order)
        );
    }

    [HttpGet("{bill_id}/orders")]
    public async Task<ActionResult<List<OrderResponse>>> ListOrders(Guid bill_id)
    {
        var orders = await billService.ListOrders(bill_id);

        return orders
            .Select(OrderResponse.FromModel)
            .ToList();
    }

    [HttpGet("{bill_id}/orders/{order_id}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(Guid bill_id, short order_id)
    {
        var order = await billService.GetOrder(bill_id, order_id);

        if (order is null)
        {
            return NotFound();
        }

        return OrderResponse.FromModel(order);
    }

    [HttpPost("{bill_id}/orders/{order_id}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateOrderStatus(Guid bill_id, short order_id, OrderStatusRequest body)
    {
        var order = await billService.GetOrder(bill_id, order_id);

        if (order is null)
        {
            return NotFound();
        }

        await billService.UpdateOrderStatus(order, body.status);
        await billService.SaveAsync();

        return OrderResponse.FromModel(order);
    }

    [HttpPost("{bill_id}/orders/{order_id}/items")]
    public async Task<ActionResult> SetOrderItem(Guid bill_id, short order_id, OrderItemRequest body)
    {
        var order = await billService.GetOrder(bill_id, order_id);

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

    [HttpDelete("{bill_id}/orders/{order_id}")]
    public async Task<ActionResult> DeleteOrder(Guid bill_id, short order_id)
    {
        var order = await billService.GetOrder(bill_id, order_id);

        if (order is null)
        {
            return NotFound();
        }

        await billService.DeleteOrder(order);
        await billService.SaveAsync();

        return NoContent();
    }
}