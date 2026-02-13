namespace FoodSphere.Pos.Api.Controller;

[Route("bills")]
public class BillController(
    ILogger<BillController> logger,
    BillService billService,
    MenuService menuService,
    BranchService branchService,
    OrderingPortalService orderingPortalService
) : PosControllerBase
{
    /// <summary>
    /// create bill
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BillResponse>> CreateBill(BillRequest body)
    {
        var branch = await branchService.GetBranch(
            restaurantId: body.restaurant_id,
            branchId: body.branch_id
        );

        if (branch is null)
        {
            return NotFound();
        }

        // if (UserType is UserType.Master)
        // {
        //     if (!await branchService.CheckPermissions(branch, MasterUser))
        //     {
        //         return Forbid();
        //     }
        // }
        // else if (UserType is UserType.Staff)
        // {
        //     if (!await branchService.CheckPermissions(branch, StaffUser))
        //     {
        //         return Forbid();
        //     }
        // }

        var table = await branchService.GetTable(
            restaurantId: body.restaurant_id,
            branchId: body.branch_id,
            tableId: body.table_id
        );

        if (table is null)
        {
            return NotFound();
        }

        var bill = await billService.CreateBill(
            table: table,
            pax: body.pax,
            consumerId: body.consumer_id
        );

        await billService.SaveChanges();

        return CreatedAtAction(
            nameof(GetBill),
            new { bill_id = bill.Id },
            BillResponse.FromModel(bill)
        );
    }

    /// <summary>
    /// get bill
    /// </summary>
    [HttpGet("{bill_id}")]
    public async Task<ActionResult<BillResponse>> GetBill(Guid bill_id)
    {
        var bill = await billService.GetBill(bill_id);

        if (bill is null) return NotFound();

        var branch = await branchService.GetBranch(
            restaurantId: bill.RestaurantId,
            branchId: bill.BranchId
        );

        if (branch is null)
        {
            return NotFound();
        }

        // if (UserType is UserType.Master)
        // {
        //     if (!await branchService.CheckPermissions(branch, MasterUser))
        //     {
        //         return Forbid();
        //     }
        // }
        // else if (UserType is UserType.Staff)
        // {
        //     if (!await branchService.CheckPermissions(branch, StaffUser))
        //     {
        //         return Forbid();
        //     }
        // }

        return BillResponse.FromModel(bill);
    }

    /// <summary>
    /// create ordering's portal
    /// </summary>
    [HttpPost("{bill_id}/portals")]
    public async Task<ActionResult<OrderingPortalResponse>> CreatePortal(Guid bill_id, OrderingPortalRequest body)
    {
        var portal = await orderingPortalService.CreatePortal(
            bill_id,
            body.max_usage,
            body.valid_duration
        );

        await orderingPortalService.SaveChanges();

        return OrderingPortalResponse.FromModel(portal);

        // return CreatedAtAction(
        //     nameof(GetPortal),
        //     new { portal_id = portal.Id },
        //     OrderingPortalResponse.FromModel(portal)
        // );
    }

    /// <summary>
    /// list ordering's portals
    /// </summary>
    [HttpGet("{bill_id}/portals")]
    public async Task<ActionResult<List<OrderingPortalResponse>>> ListPortals(Guid bill_id, [FromQuery] Guid? portal_id)
    {
        var portals = await orderingPortalService.ListPortals(bill_id);

        if (portal_id is not null)
        {
            portals = [.. portals.Where(p => p.Id == portal_id)];
        }

        return portals
            .Select(OrderingPortalResponse.FromModel)
            .ToList();
    }

    // [HttpPut("{bill_id}")]
    // public async Task<ActionResult> UpdateBill(Guid bill_id, BillRequest bill)
    // {
    //     return NoContent();
    // }

    /// <summary>
    /// delete bill
    /// </summary>
    [HttpDelete("{bill_id}")]
    public async Task<ActionResult> DeleteBill(Guid bill_id)
    {
        var bill = await billService.GetBill(bill_id);

        if (bill is null)
        {
            return NotFound();
        }

        await billService.DeleteBill(bill);
        await billService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// create order
    /// </summary>
    [HttpPost("{bill_id}/orders")]
    public async Task<ActionResult<OrderResponse>> CreateOrder(Guid bill_id, OrderDto body)
    {
        var bill = await billService.GetBill(bill_id);

        if (bill is null)
        {
            return NotFound();
        }

        var order = await billService.CreateOrder(bill);

        foreach (var item in body.items)
        {
            var menu = await menuService.FindMenu(bill.RestaurantId, item.menu_id);

            if (menu is null)
            {
                return NotFound();
            }

            await billService.CreateOrderItem(order, menu, item.quantity, item.note);
        }

        await billService.SaveChanges();

        return CreatedAtAction(
            nameof(GetOrder),
            new { bill_id, order_id = order.Id },
            OrderResponse.FromModel(order)
        );
    }

    /// <summary>
    /// list orders
    /// </summary>
    [HttpGet("{bill_id}/orders")]
    public async Task<ActionResult<List<OrderResponse>>> ListOrders(Guid bill_id)
    {
        var orders = await billService.ListOrders(bill_id);

        return orders
            .Select(OrderResponse.FromModel)
            .ToList();
    }

    /// <summary>
    /// get order
    /// </summary>
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

    /// <summary>
    /// update order status
    /// </summary>
    [HttpPost("{bill_id}/orders/{order_id}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateOrderStatus(Guid bill_id, short order_id, OrderStatusDTO body)
    {
        var order = await billService.GetOrder(bill_id, order_id);

        if (order is null)
        {
            return NotFound();
        }

        await billService.UpdateOrderStatus(order, body.status);
        await billService.SaveChanges();

        return OrderResponse.FromModel(order);
    }

    /// <summary>
    /// add item to order
    /// </summary>
    [HttpPost("{bill_id}/orders/{order_id}/items")]
    public async Task<ActionResult> CreateOrderItem(Guid bill_id, short order_id, OrderItemDto body)
    {
        var order = await billService.GetOrder(bill_id, order_id);

        if (order is null)
        {
            return NotFound();
        }

        var menu = await menuService.FindMenu(order.Bill.RestaurantId, body.menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        await billService.CreateOrderItem(order, menu, body.quantity, body.note);
        await billService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// delete order
    /// </summary>
    [HttpDelete("{bill_id}/orders/{order_id}")]
    public async Task<ActionResult> DeleteOrder(Guid bill_id, short order_id)
    {
        var order = await billService.GetOrder(bill_id, order_id);

        if (order is null)
        {
            return NotFound();
        }

        await billService.DeleteOrder(order);
        await billService.SaveChanges();

        return NoContent();
    }
}