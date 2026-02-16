namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/bills")]
public class SingleBillController(
    ILogger<SingleBillController> logger,
    BillService billService,
    MenuService menuService,
    BranchService branchService,
    OrderingPortalService orderingPortalService
) : PosControllerBase
{
    /// <summary>
    /// list bills in this branch
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<BillResponse>>> ListBills(Guid restaurant_id)
    {
        var responses = await billService.QueryBills()
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1)
            .Select(BillResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create bill
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BillResponse>> CreateBill(Guid restaurant_id, BillRequest body)
    {
        var table = branchService.GetTableStub(restaurant_id, 1, body.table_id);

        var bill = await billService.CreateBill(
            table: table,
            pax: body.pax,
            consumerId: body.consumer_id
        );

        await billService.SaveChanges();

        var response = await billService.GetBill(bill.Id, BillResponse.Projection);

        return CreatedAtAction(
            nameof(GetBill),
            new { restaurant_id, bill_id = bill.Id },
            response);
    }

    /// <summary>
    /// get bill
    /// </summary>
    [HttpGet("{bill_id}")]
    public async Task<ActionResult<BillResponse>> GetBill(Guid restaurant_id, Guid bill_id)
    {
        var response = await billService.QuerySingleBill(bill_id)
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1)
            .Select(BillResponse.Projection)
            .SingleOrDefaultAsync();

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    [HttpPut("{bill_id}")]
    public async Task<ActionResult> UpdateBill(Guid restaurant_id, Guid bill_id, BillRequest body)
    {
        var foundBill = await billService.QuerySingleBill(bill_id)
            .AnyAsync(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1);

        if (!foundBill)
        {
            return NotFound();
        }

        var bill = billService.GetBillStub(bill_id);

        bill.TableId = body.table_id;
        bill.ConsumerId = body.consumer_id;
        bill.Pax = body.pax;

        await billService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// delete bill
    /// </summary>
    [HttpDelete("{bill_id}")]
    public async Task<ActionResult> DeleteBill(Guid restaurant_id, Guid bill_id)
    {
        var affected = await billService.QuerySingleBill(bill_id)
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// list ordering's portals
    /// </summary>
    [HttpGet("{bill_id}/portals")]
    public async Task<ActionResult<ICollection<OrderingPortalResponse>>> ListPortals(
        Guid restaurant_id, Guid bill_id)
    {
        var responses = await orderingPortalService.QueryPortals()
            .Where(e =>
                e.BillId == bill_id &&
                e.Bill.RestaurantId == restaurant_id &&
                e.Bill.BranchId == 1)
            .Select(OrderingPortalResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create ordering's portal
    /// </summary>
    [HttpPost("{bill_id}/portals")]
    public async Task<ActionResult<OrderingPortalResponse>> CreatePortal(
        Guid restaurant_id,
        Guid bill_id, OrderingPortalRequest body)
    {
        var foundBill = await billService.QuerySingleBill(bill_id)
            .AnyAsync(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1);

        if (!foundBill)
        {
            return NotFound();
        }

        var portal = await orderingPortalService.CreatePortal(
            billId: bill_id,
            maxUsage: body.max_usage,
            validDuration: body.valid_duration
        );

        await orderingPortalService.SaveChanges();

        var response = OrderingPortalResponse.Project(portal);

        return CreatedAtAction(
            nameof(GetPortal),
            new { restaurant_id, bill_id, portal_id = portal.Id },
            response);
    }

    /// <summary>
    /// get ordering's portal
    /// </summary>
    [HttpGet("{bill_id}/portals/{portal_id}")]
    public async Task<ActionResult<OrderingPortalResponse>> GetPortal(
        Guid restaurant_id,
        Guid bill_id, Guid portal_id)
    {
        var response = await orderingPortalService.QuerySinglePortal(portal_id)
            .Where(e =>
                e.BillId == bill_id &&
                e.Bill.RestaurantId == restaurant_id &&
                e.Bill.BranchId == 1)
            .Select(OrderingPortalResponse.Projection)
            .SingleOrDefaultAsync();

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// list orders
    /// </summary>
    [HttpGet("{bill_id}/orders")]
    public async Task<ActionResult<ICollection<OrderResponse>>> ListOrders(
        Guid restaurant_id, Guid bill_id)
    {
        var responses = await billService.QueryOrders(bill_id)
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1)
            .Select(OrderResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create order
    /// </summary>
    [HttpPost("{bill_id}/orders")]
    public async Task<ActionResult<OrderResponse>> CreateOrder(
        Guid restaurant_id,
        Guid bill_id, OrderRequest body)
    {
        await using var transaction = await billService.GetTransaction();

        var foundBill = await billService.QuerySingleBill(bill_id)
            .AnyAsync(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1);

        if (!foundBill)
        {
            return NotFound();
        }

        var bill = billService.GetBillStub(bill_id);
        var order = await billService.CreateOrder(bill);

        foreach (var item in body.items)
        {
            var menu = menuService.GetMenuStub(restaurant_id, item.menu_id);
            await billService.CreateItem(order, menu, item.quantity, item.note);
        }

        // try catch if menu not found
        await billService.SaveChanges();
        await transaction.CommitAsync();

        var response = await billService.GetOrder(bill_id, order.Id, OrderResponse.Projection);

        return CreatedAtAction(
            nameof(GetOrder),
            new { restaurant_id, bill_id, order_id = order.Id },
            response);
    }

    /// <summary>
    /// get order
    /// </summary>
    [HttpGet("{bill_id}/orders/{order_id}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(
        Guid restaurant_id,
        Guid bill_id, short order_id)
    {
        var response = await billService.QuerySingleOrder(bill_id, order_id)
            .Where(e =>
                e.Bill.RestaurantId == restaurant_id &&
                e.Bill.BranchId == 1)
            .Select(OrderResponse.Projection)
            .SingleOrDefaultAsync();

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// update order status
    /// </summary>
    [HttpPut("{bill_id}/orders/{order_id}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateOrderStatus(
        Guid restaurant_id,
        Guid bill_id, short order_id,
        OrderStatusRequest body)
    {
        var found = await billService.QuerySingleOrder(bill_id, order_id)
            .AnyAsync(e =>
                e.Bill.RestaurantId == restaurant_id &&
                e.Bill.BranchId == 1);

        if (!found)
        {
            return NotFound();
        }

        var order = billService.GetOrderStub(bill_id, order_id);

        order.Status = body.status;

        await billService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// delete order
    /// </summary>
    [HttpDelete("{bill_id}/orders/{order_id}")]
    public async Task<ActionResult> DeleteOrder(Guid restaurant_id, Guid bill_id, short order_id)
    {
        var affected = await billService.QuerySingleOrder(bill_id, order_id)
            .Where(e =>
                e.Bill.RestaurantId == restaurant_id &&
                e.Bill.BranchId == 1)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// list order's items
    /// </summary>
    [HttpGet("{bill_id}/orders/{order_id}/items")]
    public async Task<ActionResult<ICollection<OrderItemResponse>>> ListOrderItems(
        Guid restaurant_id,
        Guid bill_id, short order_id)
    {
        var responses = await billService.QueryItems(bill_id, order_id)
            .Where(e =>
                e.Order.Bill.RestaurantId == restaurant_id &&
                e.Order.Bill.BranchId == 1)
            .Select(OrderItemResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// update order's items
    /// </summary>
    [HttpPut("{bill_id}/orders/{order_id}/items")]
    public async Task<ActionResult> UpdateOrderItems(
        Guid restaurant_id,
        Guid bill_id, short order_id,
        IReadOnlyCollection<OrderItemRequest> body)
    {
        var foundOrder = await billService.QuerySingleOrder(bill_id, order_id)
            .AnyAsync(e =>
                e.Bill.RestaurantId == restaurant_id &&
                e.Bill.BranchId == 1);

        if (!foundOrder)
        {
            return NotFound();
        }

        await using var transaction = await billService.GetTransaction();

        await billService
            .QueryItems(bill_id, order_id)
            .ExecuteDeleteAsync();

        var order = billService.GetOrderStub(bill_id, order_id);

        foreach (var item in body)
        {
            var menu = menuService.GetMenuStub(restaurant_id, item.menu_id);
            await billService.CreateItem(order, menu, item.quantity, item.note);
        }

        await billService.SaveChanges();
        await transaction.CommitAsync();

        return NoContent();
    }

    /// <summary>
    /// add item to order
    /// </summary>
    [HttpPost("{bill_id}/orders/{order_id}/items")]
    public async Task<ActionResult> CreateOrderItem(
        Guid restaurant_id,
        Guid bill_id, short order_id,
        OrderItemRequest body)
    {
        var foundOrder = await billService.QuerySingleOrder(bill_id, order_id)
            .AnyAsync(e =>
                e.Bill.RestaurantId == restaurant_id &&
                e.Bill.BranchId == 1);

        if (!foundOrder)
        {
            return NotFound();
        }

        var order = billService.GetOrderStub(bill_id, order_id);
        var menu = menuService.GetMenuStub(restaurant_id, body.menu_id);
        var item = await billService.CreateItem(order, menu, body.quantity, body.note);

        await billService.SaveChanges();

        var response = await billService.GetItem(bill_id, order_id, body.menu_id, OrderItemResponse.Projection);

        return CreatedAtAction(
            nameof(GetOrderItem),
            new { restaurant_id, bill_id, order_id, item_id = item.Id },
            response);
    }

    /// <summary>
    /// get specific item
    /// </summary>
    [HttpGet("{bill_id}/orders/{order_id}/items/{item_id}")]
    public async Task<ActionResult<OrderItemResponse>> GetOrderItem(
        Guid restaurant_id,
        Guid bill_id, short order_id, short item_id)
    {
        var response = await billService.QuerySingleItem(bill_id, order_id, item_id)
            .Where(e =>
                e.Order.Bill.RestaurantId == restaurant_id &&
                e.Order.Bill.BranchId == 1)
            .Select(OrderItemResponse.Projection)
            .SingleOrDefaultAsync();

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// update specific item
    /// </summary>
    [HttpPut("{bill_id}/orders/{order_id}/items/{item_id}")]
    public async Task<ActionResult<OrderItemResponse>> UpdateOrderItem(
        Guid restaurant_id,
        Guid bill_id, short order_id, short item_id,
        OrderItemUpdateRequest body)
    {
        var found = await billService.QuerySingleItem(bill_id, order_id, item_id)
            .AnyAsync(e =>
                e.Order.Bill.RestaurantId == restaurant_id &&
                e.Order.Bill.BranchId == 1);

        if (!found)
        {
            return NotFound();
        }

        var item = billService.GetItemStub(bill_id, order_id, item_id);

        item.Quantity = body.quantity;
        item.Note = body.note;

        await billService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// remove specific item
    /// </summary>
    [HttpDelete("{bill_id}/orders/{order_id}/items/{item_id}")]
    public async Task<ActionResult> DeleteOrderItem(
        Guid restaurant_id,
        Guid bill_id, short order_id, short item_id)
    {
        var affected = await billService.QuerySingleItem(bill_id, order_id, item_id)
            .Where(e =>
                e.Order.Bill.RestaurantId == restaurant_id &&
                e.Order.Bill.BranchId == 1)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }
}