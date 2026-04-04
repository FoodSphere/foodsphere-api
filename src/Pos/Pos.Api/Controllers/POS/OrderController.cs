namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/branches/{branch_id}")]
public class OrderController(
    ILogger<BillController> logger,
    OrderServiceBase orderService,
    AccessControlService accessControl
) : PosControllerBase
{
    /// <summary>
    /// list orders
    /// </summary>
    [HttpGet("orders")]
    public async Task<ActionResult<ICollection<OrderResponse>>> ListOrders(
        Guid restaurant_id, short branch_id,
        [FromQuery] IReadOnlyCollection<OrderStatus> order_status,
        [FromQuery] IReadOnlyCollection<BillStatus> bill_status,
        [FromQuery] bool? is_deleted = false)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Order.LIST);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        Expression<Func<Order, bool>> predicate = e =>
            e.Bill.RestaurantId == restaurant_id &&
            e.Bill.BranchId == branch_id;

        if (is_deleted is not null)
            predicate = predicate.And(e => e.DeleteTime != null == is_deleted.Value);

        if (order_status.Count != 0)
            predicate = predicate.And(e => order_status.Contains(e.Status));

        if (bill_status.Count != 0)
            predicate = predicate.And(e => bill_status.Contains(e.Bill.Status));

        return await orderService.ListOrders(
            OrderResponse.Projection, predicate);
    }

    /// <summary>
    /// list orders in bills
    /// </summary>
    [HttpGet("bills/{bill_id}/orders")]
    public async Task<ActionResult<ICollection<OrderResponse>>> ListOrders(
        Guid restaurant_id, short branch_id, Guid bill_id,
        [FromQuery] IReadOnlyCollection<OrderStatus> status,
        [FromQuery] bool? is_deleted = false)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Order.LIST);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        Expression<Func<Order, bool>> predicate = e =>
            e.BillId == bill_id &&
            e.Bill.RestaurantId == restaurant_id &&
            e.Bill.BranchId == branch_id;

        if (is_deleted is not null)
            predicate = predicate.And(e => e.DeleteTime != null == is_deleted.Value);

        if (status.Count != 0)
            predicate = predicate.And(e => status.Contains(e.Status));

        return await orderService.ListOrders(
            OrderResponse.Projection, predicate);
    }

    /// <summary>
    /// create order
    /// </summary>
    [HttpPost("bills/{bill_id}/orders")]
    public async Task<ActionResult<OrderResponse>> CreateOrder(
        Guid restaurant_id, short branch_id,
        Guid bill_id, OrderRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Order.CREATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var items = body.items.Select(e =>
            new OrderItemCreateCommand(
                new(restaurant_id, e.menu_id),
                e.quantity,
                e.note));

        var result = await orderService.CreateOrder(
            OrderResponse.Projection,
            new(bill_id),
            new(items, body.status));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (key, response) = result.Value;

        return CreatedAtAction(
            nameof(GetOrder),
            new { restaurant_id, branch_id, bill_id, order_id = key.Id },
            response);
    }

    /// <summary>
    /// get order
    /// </summary>
    [HttpGet("bills/{bill_id}/orders/{order_id}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(
        Guid restaurant_id, short branch_id,
        Guid bill_id, short order_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Order.GET);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var response = await orderService.GetOrder(
            OrderResponse.Projection,
            new(bill_id, order_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// delete order
    /// </summary>
    [HttpDelete("bills/{bill_id}/orders/{order_id}")]
    public async Task<ActionResult> DeleteOrder(
        Guid restaurant_id, short branch_id, Guid bill_id, short order_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Order.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await orderService.DeleteOrder(
            new(bill_id, order_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// update order status
    /// </summary>
    [HttpPut("bills/{bill_id}/orders/{order_id}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateOrderStatus(
        Guid restaurant_id, short branch_id,
        Guid bill_id, short order_id,
        OrderStatusRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Order.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await orderService.UpdateOrderStatus(
            new(bill_id, order_id),
            body.status);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// list order's items
    /// </summary>
    [HttpGet("bills/{bill_id}/orders/{order_id}/items")]
    public async Task<ActionResult<ICollection<OrderItemResponse>>> ListOrderItems(
        Guid restaurant_id, short branch_id,
        Guid bill_id, short order_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Order.GET);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        return await orderService.ListItems(
            OrderItemResponse.Projection, e =>
                e.BillId == bill_id &&
                e.Bill.RestaurantId == restaurant_id &&
                e.Bill.BranchId == branch_id &&
                e.OrderId == order_id);
    }

    /// <summary>
    /// add item to order
    /// </summary>
    [HttpPost("bills/{bill_id}/orders/{order_id}/items")]
    public async Task<ActionResult<OrderItemResponse>> CreateOrderItem(
        Guid restaurant_id, short branch_id,
        Guid bill_id, short order_id,
        OrderItemRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Order.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await orderService.CreateItem(
            OrderItemResponse.Projection,
            new(bill_id, order_id),
            new(restaurant_id, body.menu_id),
            body.quantity,
            body.note);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (key, response) = result.Value;

        return CreatedAtAction(
            nameof(GetOrderItem),
            new { restaurant_id, branch_id, bill_id, order_id, item_id = key.Id },
            response);
    }

    /// <summary>
    /// update order's items
    /// </summary>
    [HttpPut("bills/{bill_id}/orders/{order_id}/items")]
    public async Task<ActionResult> UpdateOrderItems(
        Guid restaurant_id, short branch_id,
        Guid bill_id, short order_id,
        IReadOnlyCollection<OrderItemRequest> body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Order.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var command = body.Select(e =>
            new OrderItemCreateCommand(
                new(restaurant_id, e.menu_id),
                e.quantity,
                e.note));

        var result = await orderService.SetOrderItems(
            new(bill_id, order_id),
            command);

        return NoContent();
    }

    /// <summary>
    /// get specific item
    /// </summary>
    [HttpGet("bills/{bill_id}/orders/{order_id}/items/{item_id}")]
    public async Task<ActionResult<OrderItemResponse>> GetOrderItem(
        Guid restaurant_id, short branch_id,
        Guid bill_id, short order_id, short item_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Order.GET);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var response = await orderService.GetItem(
            OrderItemResponse.Projection,
            new(bill_id, order_id, item_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// update specific item
    /// </summary>
    [HttpPut("bills/{bill_id}/orders/{order_id}/items/{item_id}")]
    public async Task<ActionResult<OrderItemResponse>> UpdateOrderItem(
        Guid restaurant_id, short branch_id,
        Guid bill_id, short order_id, short item_id,
        OrderItemUpdateRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Order.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await orderService.UpdateItem(
            new(bill_id, order_id, item_id),
            new(body.quantity, body.note));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// remove specific item
    /// </summary>
    [HttpDelete("bills/{bill_id}/orders/{order_id}/items/{item_id}")]
    public async Task<ActionResult> DeleteOrderItem(
        Guid restaurant_id, short branch_id,
        Guid bill_id, short order_id, short item_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Order.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await orderService.DeleteItem(
            new(bill_id, order_id, item_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}