namespace FoodSphere.Pos.Api.Controllers;

[Route("bills")]
public class BillController(
    ILogger<BillController> logger,
    BillService billService,
    MenuService menuService,
    BranchService branchService,
    OrderingPortalService orderingPortalService
) : PosControllerBase
{
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

        if (UserType is UserType.Master)
        {
            if (!await branchService.CheckPermissions(branch, MasterUser))
            {
                return Forbid();
            }
        }
        else if (UserType is UserType.Staff)
        {
            if (!await branchService.CheckPermissions(branch, StaffUser))
            {
                return Forbid();
            }
        }

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

        var branch = await branchService.GetBranch(
            restaurantId: bill.RestaurantId,
            branchId: bill.BranchId
        );

        if (branch is null)
        {
            return NotFound();
        }

        if (UserType is UserType.Master)
        {
            if (!await branchService.CheckPermissions(branch, MasterUser))
            {
                return Forbid();
            }
        }
        else if (UserType is UserType.Staff)
        {
            if (!await branchService.CheckPermissions(branch, StaffUser))
            {
                return Forbid();
            }
        }

        return BillResponse.FromModel(bill);
    }

    [HttpPost("{bill_id}/portals")]
    public async Task<ActionResult<OrderingPortalResponse>> CreatePortal(Guid bill_id, OrderingPortalRequest body)
    {
        var portal = await orderingPortalService.CreatePortal(
            bill_id,
            body.max_usage,
            body.valid_duration
        );

        await orderingPortalService.SaveAsync();

        return OrderingPortalResponse.FromModel(portal);

        // return CreatedAtAction(
        //     nameof(GetPortal),
        //     new { portal_id = portal.Id },
        //     OrderingPortalResponse.FromModel(portal)
        // );
    }

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
            var menu = await menuService.GetMenu(bill.RestaurantId, item.menu_id);

            if (menu is null)
            {
                return NotFound();
            }

            await billService.SetOrderItem(order, menu, item.quantity);
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
    public async Task<ActionResult<OrderResponse>> UpdateOrderStatus(Guid bill_id, short order_id, OrderStatusDTO body)
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
    public async Task<ActionResult> SetOrderItem(Guid bill_id, short order_id, OrderItemDto body)
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

        await billService.SetOrderItem(order, menu, body.quantity);
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