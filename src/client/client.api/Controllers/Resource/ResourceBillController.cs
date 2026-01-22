using Microsoft.AspNetCore.Mvc;
using FoodSphere.Services;
using FoodSphere.Data;
using FoodSphere.Data.Models;

namespace FoodSphere.Controllers.Resource;

public class ResourceBillRequest
{
    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }
    public short table_id { get; set; }

    public Guid? consumer_id { get; set; }
    public short? pax { get; set; }
}

public class ResourceBillResponse
{
    public Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid? consumer_id { get; set; }
    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }
    public short table_id { get; set; }

    public List<ResourceOrderResponse> orders { get; set; } = [];

    public short? pax { get; set; }
    public BillStatus status { get; set; }

    public static ResourceBillResponse FromModel(Bill model)
    {
        return new ResourceBillResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            consumer_id = model.ConsumerId,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            table_id = model.TableId,
            orders = [.. model.Orders.Select(ResourceOrderResponse.FromModel)],
            pax = model.Pax,
            status = model.Status,
        };
    }
}

public class ResourceOrderDTO
{
    public List<ResourceOrderItemDTO> items { get; set; } = [];
}

public class ResourceOrderResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid bill_id { get; set; }

    public List<ResourceOrderItemDTO> items { get; set; } = [];

    public OrderStatus status { get; set; }

    public static ResourceOrderResponse FromModel(Order model)
    {
        return new ResourceOrderResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            bill_id = model.BillId,
            items = [.. model.Items.Select(ResourceOrderItemDTO.FromModel)],
            status = model.Status,
        };
    }
}

public class ResourceOrderItemDTO
{
    public short menu_id { get; set; }
    public short quantity { get; set; }
    public string? note { get; set; }

    public static ResourceOrderItemDTO FromModel(OrderItem model)
    {
        return new ResourceOrderItemDTO
        {
            menu_id = model.MenuId,
            quantity = model.Quantity,
            note = model.Note,
        };
    }
}

public class ResourceOrderStatusDTO
{
    public OrderStatus status { get; set; }
}

[Route("resource/bills")]
public class ResourceBillController(
    ILogger<ResourceBillController> logger,
    BillService billService,
    MenuService menuService,
    BranchService branchService
) : AdminController
{
    readonly ILogger<ResourceBillController> _logger = logger;
    readonly BillService _billService = billService;
    readonly MenuService _menuService = menuService;
    readonly BranchService _branchService = branchService;

    [HttpPost]
    public async Task<ActionResult<ResourceBillResponse>> CreateBill(ResourceBillRequest body)
    {
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
            consumerId: body.consumer_id,
            table: table,
            pax: body.pax
        );

        await _billService.Save();

        return CreatedAtAction(
            nameof(GetBill),
            new { bill_id = bill.Id },
            ResourceBillResponse.FromModel(bill)
        );
    }

    [HttpGet("{bill_id}")]
    public async Task<ActionResult<ResourceBillResponse>> GetBill(Guid bill_id)
    {
        var bill = await _billService.GetBill(bill_id);

        if (bill is null) return NotFound();

        return ResourceBillResponse.FromModel(bill);
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
    public async Task<ActionResult<ResourceOrderResponse>> CreateOrder(Guid bill_id, ResourceOrderDTO body)
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
            ResourceOrderResponse.FromModel(order)
        );
    }

    [HttpGet("{bill_id}/orders")]
    public async Task<ActionResult<List<ResourceOrderResponse>>> ListOrders(Guid bill_id)
    {
        var orders = await _billService.ListOrders(bill_id);

        return orders
            .Select(ResourceOrderResponse.FromModel)
            .ToList();
    }

    [HttpGet("{bill_id}/orders/{order_id}")]
    public async Task<ActionResult<ResourceOrderResponse>> GetOrder(Guid bill_id, short order_id)
    {
        var order = await _billService.GetOrder(bill_id, order_id);

        if (order is null)
        {
            return NotFound();
        }

        return ResourceOrderResponse.FromModel(order);
    }

    [HttpPost("{bill_id}/orders/{order_id}/status")]
    public async Task<ActionResult<ResourceOrderResponse>> UpdateOrderStatus(Guid bill_id, short order_id, ResourceOrderStatusDTO body)
    {
        var order = await _billService.GetOrder(bill_id, order_id);

        if (order is null)
        {
            return NotFound();
        }

        await _billService.UpdateOrderStatus(order, body.status);
        await _billService.Save();

        return ResourceOrderResponse.FromModel(order);
    }

    [HttpPost("{bill_id}/orders/{order_id}/items")]
    public async Task<ActionResult> SetOrderItem(Guid bill_id, short order_id, ResourceOrderItemDTO body)
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