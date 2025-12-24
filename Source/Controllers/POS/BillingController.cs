using Microsoft.AspNetCore.Mvc;
using FoodSphere.Services;
using FoodSphere.Data;
using FoodSphere.Data.Models;

namespace FoodSphere.Controllers.POS;

public class BillRequest
{
    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }
    public short table_id { get; set; }

    public Guid? consumer_id { get; set; }
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

public class OrderDTO
{
    public List<OrderItemDTO> items { get; set; } = [];
}

public class OrderResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid bill_id { get; set; }

    public List<OrderItemDTO> items { get; set; } = [];

    public OrderStatus status { get; set; }

    public static OrderResponse FromModel(Order model)
    {
        return new OrderResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            bill_id = model.BillId,
            items = [.. model.Items.Select(OrderItemDTO.FromModel)],
            status = model.Status,
        };
    }
}

public class OrderItemDTO
{
    public short menu_id { get; set; }
    public short quantity { get; set; }
    public string? note { get; set; }

    public static OrderItemDTO FromModel(OrderItem model)
    {
        return new OrderItemDTO
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

    public short? max_usage { get; set; }
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

[Route("client/bills")]
public class BillingController(
    ILogger<BillingController> logger,
    BillService billService,
    MenuService menuService,
    BranchService branchService,
    OrderingService orderingService
) : ClientController
{
    readonly ILogger<BillingController> _logger = logger;
    readonly BillService _billService = billService;
    readonly MenuService _menuService = menuService;
    readonly BranchService _branchService = branchService;
    readonly OrderingService _orderingService = orderingService;

    [HttpPost]
    public async Task<ActionResult<BillResponse>> CreateBill(BillRequest body)
    {
        var branch = await _branchService.GetBranch(
            restaurantId: body.restaurant_id,
            branchId: body.branch_id
        );

        if (branch is null)
        {
            return NotFound();
        }

        if (UserType is UserType.Master)
        {
            if (!await _branchService.CheckPermissions(branch, MasterUser))
            {
                return Forbid();
            }
        }
        else if (UserType is UserType.Staff)
        {
            if (!await _branchService.CheckPermissions(branch, StaffUser))
            {
                return Forbid();
            }
        }

        var table = await _branchService.GetTable(
            restaurantId: body.restaurant_id,
            branchId: body.branch_id,
            tableId: body.table_id
        );

        if (table is null)
        {
            return NotFound();
        }

        var bill = await _billService.CreateBill(
            table: table,
            pax: body.pax,
            consumerId: body.consumer_id
        );

        await _billService.Save();

        return CreatedAtAction(
            nameof(GetBill),
            new { bill_id = bill.Id },
            BillResponse.FromModel(bill)
        );
    }

    [HttpGet("{bill_id}")]
    public async Task<ActionResult<BillResponse>> GetBill(Guid bill_id)
    {
        var bill = await _billService.GetBill(bill_id);

        if (bill is null) return NotFound();

        var branch = await _branchService.GetBranch(
            restaurantId: bill.RestaurantId,
            branchId: bill.BranchId
        );

        if (branch is null)
        {
            return NotFound();
        }

        if (UserType is UserType.Master)
        {
            if (!await _branchService.CheckPermissions(branch, MasterUser))
            {
                return Forbid();
            }
        }
        else if (UserType is UserType.Staff)
        {
            if (!await _branchService.CheckPermissions(branch, StaffUser))
            {
                return Forbid();
            }
        }

        return BillResponse.FromModel(bill);
    }

    [HttpPost("{bill_id}/portals")]
    public async Task<ActionResult<OrderingPortalResponse>> CreatePortal(Guid bill_id, OrderingPortalRequest body)
    {
        var portal = await _orderingService.CreatePortal(
            bill_id,
            body.max_usage,
            body.valid_duration
        );

        await _orderingService.Save();

        return OrderingPortalResponse.FromModel(portal);

        // return CreatedAtAction(
        //     nameof(GetPortal),
        //     new { portal_id = portal.Id },
        //     OrderingPortalResponse.FromModel(portal)
        // );
    }

    [HttpGet("{bill_id}/portals")]
    public async Task<ActionResult<List<OrderingPortalResponse>>> ListPortal(Guid bill_id, [FromQuery] Guid? portal_id)
    {
        var portals = await _orderingService.ListPortals(bill_id);

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
        var bill = await _billService.GetBill(bill_id);

        if (bill is null)
        {
            return NotFound();
        }

        await _billService.DeleteBill(bill);
        await _billService.Save();

        return NoContent();
    }

    [HttpPost("{bill_id}/orders")]
    public async Task<ActionResult<OrderResponse>> CreateOrder(Guid bill_id, OrderDTO body)
    {
        var bill = await _billService.GetBill(bill_id);

        if (bill is null)
        {
            return NotFound();
        }

        var order = await _billService.CreateOrder(bill);

        foreach (var item in body.items)
        {
            var menu = await _menuService.GetMenu(bill.RestaurantId, item.menu_id);

            if (menu is null)
            {
                return NotFound();
            }

            await _billService.SetOrderItem(order, menu, item.quantity);
        }

        await _billService.Save();

        return CreatedAtAction(
            nameof(GetOrder),
            new { bill_id, order_id = order.Id },
            OrderResponse.FromModel(order)
        );
    }

    [HttpGet("{bill_id}/orders")]
    public async Task<ActionResult<List<OrderResponse>>> ListOrders(Guid bill_id)
    {
        var orders = await _billService.ListOrders(bill_id);

        return orders
            .Select(OrderResponse.FromModel)
            .ToList();
    }

    [HttpGet("{bill_id}/orders/{order_id}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(Guid bill_id, short order_id)
    {
        var order = await _billService.GetOrder(bill_id, order_id);

        if (order is null)
        {
            return NotFound();
        }

        return OrderResponse.FromModel(order);
    }

    [HttpPost("{bill_id}/orders/{order_id}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateOrderStatus(Guid bill_id, short order_id, OrderStatusDTO body)
    {
        var order = await _billService.GetOrder(bill_id, order_id);

        if (order is null)
        {
            return NotFound();
        }

        await _billService.UpdateOrderStatus(order, body.status);
        await _billService.Save();

        return OrderResponse.FromModel(order);
    }

    [HttpPost("{bill_id}/orders/{order_id}/items")]
    public async Task<ActionResult> SetOrderItem(Guid bill_id, short order_id, OrderItemDTO body)
    {
        var order = await _billService.GetOrder(bill_id, order_id);

        if (order is null)
        {
            return NotFound();
        }

        var menu = await _menuService.GetMenu(order.Bill.RestaurantId, body.menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        await _billService.SetOrderItem(order, menu, body.quantity);
        await _billService.Save();

        return NoContent();
    }

    [HttpDelete("{bill_id}/orders/{order_id}")]
    public async Task<ActionResult> DeleteOrder(Guid bill_id, short order_id)
    {
        var order = await _billService.GetOrder(bill_id, order_id);

        if (order is null)
        {
            return NotFound();
        }

        await _billService.DeleteOrder(order);
        await _billService.Save();

        return NoContent();
    }
}