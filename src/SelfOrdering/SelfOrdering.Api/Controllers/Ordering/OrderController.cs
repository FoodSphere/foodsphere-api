using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

namespace FoodSphere.SelfOrdering.Api.Controller;

[Route("orders")]
public class OrderingController(
    ILogger<OrderingController> logger,
    BillService billService,
    MenuService menuService
) : SelfOrderingControllerBase
{
    /// <summary>
    /// list orders
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<OrderResponse>>> ListOrders()
    {
        var responses = await billService.QueryOrders(Member.BillId)
            .Select(OrderResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create single order
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(OrderRequest body)
    {
        var bill = billService.GetBillStub(Member.BillId);
        var restaurantId = await billService.QuerySingleBill(Member.BillId)
            .Select(e => e.RestaurantId)
            .SingleOrDefaultAsync();

        var order = await billService.CreateOrder(bill);

        foreach (var item in body.items)
        {
            var menu = menuService.GetMenuStub(restaurantId, item.menu_id);
            await billService.CreateItem(order, menu, item.quantity, item.note);
        }

        await billService.SaveChanges();

        var response = await billService.GetOrder(Member.BillId, order.Id, OrderResponse.Projection);

        return CreatedAtAction(
            nameof(GetOrder),
            new { order_id = order.Id },
            response);
    }

    /// <summary>
    /// add bulk order
    /// </summary>
    [HttpPatch]
    public async Task<ActionResult<ICollection<OrderResponse>>> PatchOrders(JsonPatchDocument<IReadOnlyCollection<OrderRequest>> body)
    {
        var bill = billService.GetBillStub(Member.BillId);
        var restaurantId = await billService.QuerySingleBill(Member.BillId)
            .Select(e => e.RestaurantId)
            .SingleOrDefaultAsync();

        var orderIds = new List<short>();
        var requests = new List<OrderRequest>();
        body.ApplyTo(requests);

        foreach (var request in requests)
        {
            var order = await billService.CreateOrder(bill);

            foreach (var item in request.items)
            {
                var menu = menuService.GetMenuStub(restaurantId, item.menu_id);
                await billService.CreateItem(order, menu, item.quantity, item.note);
            }

            orderIds.Add(order.Id);
        }

        await billService.SaveChanges();

        var responses = await billService.QueryOrders(Member.BillId)
            .Where(e => orderIds.Contains(e.Id))
            .Select(OrderResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// get order
    /// </summary>
    [HttpGet("{order_id}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(short order_id)
    {
        var response = await billService.GetOrder(Member.BillId, order_id, OrderResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// cancel order
    /// </summary>
    [HttpPut("{order_id}/cancel")]
    public async Task<ActionResult> CancelOrder(short order_id)
    {
        var order = billService.GetOrderStub(Member.BillId, order_id);

        order.Status = OrderStatus.Canceled;

        var affected = await billService.SaveChanges();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// list order's items
    /// </summary>
    [HttpGet("{order_id}/items")]
    public async Task<ActionResult<ICollection<OrderItemResponse>>> ListOrderItems(short order_id)
    {
        var responses = await billService.QueryItems(Member.BillId, order_id)
            .Select(OrderItemResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// update order's items
    /// </summary>
    [HttpPut("{order_id}/items")]
    public async Task<ActionResult> UpdateOrderItems(short order_id, IReadOnlyCollection<OrderItemRequest> body)
    {
        await using var transaction = await billService.GetTransaction();

        await billService
            .QueryItems(Member.BillId, order_id)
            .ExecuteDeleteAsync();

        var order = billService.GetOrderStub(Member.BillId, order_id);
        var restaurantId = await billService.QuerySingleBill(Member.BillId)
            .Select(e => e.RestaurantId)
            .SingleOrDefaultAsync();

        foreach (var item in body)
        {
            var menu = menuService.GetMenuStub(restaurantId, item.menu_id);
            await billService.CreateItem(order, menu, item.quantity, item.note);
        }

        await billService.SaveChanges();
        await transaction.CommitAsync();

        return NoContent();
    }

    /// <summary>
    /// add item to order
    /// </summary>
    [HttpPost("{order_id}/items")]
    public async Task<ActionResult> CreateOrderItem(short order_id, OrderItemRequest body)
    {
        var order = billService.GetOrderStub(Member.BillId, order_id);
        var restaurantId = await billService.QuerySingleBill(Member.BillId)
            .Select(e => e.RestaurantId)
            .SingleOrDefaultAsync();

        var menu = menuService.GetMenuStub(restaurantId, body.menu_id);
        var item = await billService.CreateItem(order, menu, body.quantity, body.note);

        await billService.SaveChanges();

        var response = await billService.GetItem(Member.BillId, order_id, body.menu_id, OrderItemResponse.Projection);

        return CreatedAtAction(
            nameof(GetOrderItem),
            new { order_id, item_id = item.Id },
            response);
    }

    /// <summary>
    /// get specific item
    /// </summary>
    [HttpGet("{order_id}/items/{item_id}")]
    public async Task<ActionResult<OrderItemResponse>> GetOrderItem(short order_id, short item_id)
    {
        var response = await billService.GetItem(Member.BillId, order_id, item_id, OrderItemResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// update specific item
    /// </summary>
    [HttpPut("{order_id}/items/{item_id}")]
    public async Task<ActionResult<OrderItemResponse>> UpdateOrderItem(short order_id, short item_id, OrderItemUpdateRequest body)
    {
        var item = billService.GetItemStub(Member.BillId, order_id, item_id);

        item.Quantity = body.quantity;
        item.Note = body.note;

        await billService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// remove specific item
    /// </summary>
    [HttpDelete("{order_id}/items/{item_id}")]
    public async Task<ActionResult> DeleteOrderItem(short order_id, short item_id)
    {
        var affected = await billService.QuerySingleItem(Member.BillId, order_id, item_id)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }
}