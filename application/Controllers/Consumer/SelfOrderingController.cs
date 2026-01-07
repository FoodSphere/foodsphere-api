using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using FoodSphere.Services;
using FoodSphere.Data;
using FoodSphere.Data.Models;
using FoodSphere.Configurations;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FoodSphere.Controllers.Consumer;

public class SelfOrderingPortalRequest
{
    public short? max_usage { get; set; }
    public TimeSpan? valid_duration { get; set; }
}

public class SelfOrderingPortalResponse
{
    public Guid id { get; set; }
    public Guid bill_id { get; set; }
    // public Guid? consumer_id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public short? max_usage { get; set; }
    public short usage_count { get; set; }
    public TimeSpan? valid_duration { get; set; }

    public static SelfOrderingPortalResponse FromModel(SelfOrderingPortal model)
    {
        return new SelfOrderingPortalResponse
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

public class SelfOrderingMemberDetailsRequest
{
    public string? name { get; set; }
}

public class SelfOrderingMemberDetailsResponse
{
    public short id { get; set; }
    public string? name { get; set; }

    public static SelfOrderingMemberDetailsResponse FromModel(BillMember model)
    {
        return new SelfOrderingMemberDetailsResponse
        {
            id = model.Id,
            name = model.Name,
        };
    }
}

public class SelfOrderingOrderDTO
{
    public List<SelfOrderingOrderItemDTO> items { get; set; } = [];
}

public class SelfOrderingOrderResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public List<SelfOrderingOrderItemDTO> items { get; set; } = [];

    public OrderStatus status { get; set; }

    public static SelfOrderingOrderResponse FromModel(Order model)
    {
        return new SelfOrderingOrderResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            items = [.. model.Items.Select(SelfOrderingOrderItemDTO.FromModel)],
            status = model.Status,
        };
    }
}

public class SelfOrderingOrderItemDTO
{
    public short menu_id { get; set; }
    public short quantity { get; set; }
    public string? note { get; set; }

    public static SelfOrderingOrderItemDTO FromModel(OrderItem model)
    {
        return new SelfOrderingOrderItemDTO
        {
            menu_id = model.MenuId,
            quantity = model.Quantity,
            note = model.Note,
        };
    }
}

public class SelfOrderingOrderStatusDTO
{
    public OrderStatus status { get; set; }
}

[Route("self-ordering")]
public class SelfOrderingController(
    ILogger<SelfOrderingController> logger,
    BillService billService,
    MenuService menuService,
    OrderingService orderingService
) : BaseSelfOrderingController
{
    readonly ILogger<SelfOrderingController> _logger = logger;
    readonly BillService _billService = billService;
    readonly MenuService _menuService = menuService;
    readonly OrderingService _orderingService = orderingService;

    [HttpPost("portals")]
    public async Task<ActionResult<SelfOrderingPortalResponse>> CreatePortal(SelfOrderingPortalRequest body)
    {
        var portal = await _orderingService.CreatePortal(
            BillMember.BillId,
            body.max_usage,
            body.valid_duration
        );

        await _orderingService.Save();

        return CreatedAtAction(
            nameof(ListPortal),
            new { portal_id = portal.Id },
            SelfOrderingPortalResponse.FromModel(portal)
        );
    }

    [HttpGet("portals")]
    public async Task<ActionResult<List<SelfOrderingPortalResponse>>> ListPortal([FromQuery] Guid? portal_id)
    {
        var portals = await _orderingService.ListPortals(BillMember.BillId);

        if (portal_id is not null)
        {
            portals = [.. portals.Where(p => p.Id == portal_id)];
        }

        return portals
            .Select(SelfOrderingPortalResponse.FromModel)
            .ToList();
    }

    [HttpGet("me")]
    public async Task<ActionResult<SelfOrderingMemberDetailsResponse>> GetMemberDetails()
    {
        return SelfOrderingMemberDetailsResponse.FromModel(BillMember);
    }

    [HttpPut("me")]
    public async Task<ActionResult> UpdateMemberDetails(SelfOrderingMemberDetailsRequest body)
    {
        BillMember.Name = body.name;

        await _billService.Save();

        return NoContent();
    }

    [HttpPost("orders")]
    public async Task<ActionResult<SelfOrderingOrderResponse>> CreateOrder(SelfOrderingOrderDTO body)
    {
        var bill = await _billService.GetBill(BillMember.BillId);

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
            new { order_id = order.Id },
            SelfOrderingOrderResponse.FromModel(order)
        );
    }

    [HttpGet("orders")]
    public async Task<ActionResult<List<SelfOrderingOrderResponse>>> ListOrders()
    {
        var orders = await _billService.ListOrders(BillMember.BillId);

        return orders
            .Select(SelfOrderingOrderResponse.FromModel)
            .ToList();
    }

    [HttpGet("orders/{order_id}")]
    public async Task<ActionResult<SelfOrderingOrderResponse>> GetOrder(short order_id)
    {
        var order = await _billService.GetOrder(BillMember.BillId, order_id);

        if (order is null)
        {
            return NotFound();
        }

        return SelfOrderingOrderResponse.FromModel(order);
    }

    [HttpPost("orders/{order_id}/items")]
    public async Task<ActionResult> SetOrderItem(short order_id, SelfOrderingOrderItemDTO body)
    {
        var order = await _billService.GetOrder(BillMember.BillId, order_id);

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

    [HttpDelete("orders/{order_id}")]
    public async Task<ActionResult> CancelOrder(short order_id)
    {
        var order = await _billService.GetOrder(BillMember.BillId, order_id);

        if (order is null)
        {
            return NotFound();
        }

        // check if order can be canceled ...

        await _billService.UpdateOrderStatus(order, OrderStatus.Canceled);
        await _billService.Save();

        return NoContent();
    }
}