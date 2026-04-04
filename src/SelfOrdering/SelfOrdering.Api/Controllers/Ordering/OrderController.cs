namespace FoodSphere.SelfOrdering.Api.Controller;

[Route("orders")]
public class OrderingController(
    ILogger<OrderingController> logger,
    MenuServiceBase menuService,
    OrderServiceBase orderService,
    OrderingCalculator orderingCalculator
) : SelfOrderingControllerBase
{
    /// <summary>
    /// list orders
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<OrderResponse>>> ListOrders(
        [FromQuery] IReadOnlyCollection<OrderStatus> status,
        [FromQuery] bool? is_deleted = false)
    {
        var predicate = PredicateBuilder.New<Order>(e =>
            e.BillId == MemberKey.BillId);

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
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(
        OrderRequest body)
    {
        var availabilityResult = await orderingCalculator.CalculateStockAvailability(
            body.items
                .GroupBy(e => new MenuKey(RestaurantId, e.menu_id))
                .ToDictionary(
                    e => e.Key,
                    e => (short)e.Sum(i => i.quantity)),
            BranchKey);

        if (availabilityResult.IsFailed)
            return availabilityResult.Errors.ToActionResult();

        var items = body.items.Select(e =>
            new OrderItemCreateCommand(
                new(RestaurantId, e.menu_id),
                e.quantity,
                e.note));

        var orderResult = await orderService.CreateOrder(
            OrderResponse.Projection,
            new(MemberKey.BillId),
            new(items, body.status));

        if (orderResult.IsFailed)
            return orderResult.Errors.ToActionResult();

        var (orderKey, response) = orderResult.Value;

        return CreatedAtAction(
            nameof(GetOrder),
            new { order_id = orderKey.Id },
            response);
    }

    /// <summary>
    /// add bulk order
    /// </summary>
    [HttpPatch]
    public async Task<ActionResult<ICollection<OrderResponse>>> PatchOrders(
        JsonPatchDocument<ICollection<OrderRequest>> body)
    {
        var requests = new List<OrderRequest>();
        body.ApplyTo(requests);

        var availabilityResult = await orderingCalculator.CalculateStockAvailability(
            requests.SelectMany(e => e.items)
                .GroupBy(e => new MenuKey(RestaurantId, e.menu_id))
                .ToDictionary(
                    e => e.Key,
                    e => (short)e.Sum(i => i.quantity)),
            BranchKey);

        if (availabilityResult.IsFailed)
            return availabilityResult.Errors.ToActionResult();

        var result = await orderService.CreateBulkOrders(
            OrderResponse.Projection,
            new(MemberKey.BillId),
            requests.Select(r => new OrderCreateCommand(
                r.items.Select(e =>
                    new OrderItemCreateCommand(
                        new(RestaurantId, e.menu_id),
                        e.quantity,
                        e.note)),
                r.status)));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var responses = result.Value
            .Select(i => i.Item2)
            .ToArray();

        return responses;
    }

    /// <summary>
    /// get order
    /// </summary>
    [HttpGet("{order_id}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(short order_id)
    {
        var response = await orderService.GetOrder(
            OrderResponse.Projection,
            new(MemberKey.BillId, order_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// cancel order
    /// </summary>
    [HttpPut("{order_id}/cancel")]
    public async Task<ActionResult> CancelOrder(short order_id)
    {
        var result = await orderService.UpdateOrderStatus(
            new(MemberKey.BillId, order_id),
            OrderStatus.Cancelled);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// list order's items
    /// </summary>
    [HttpGet("{order_id}/items")]
    public async Task<ActionResult<ICollection<OrderItemResponse>>> ListOrderItems(
        short order_id)
    {
        return await orderService.ListItems(
            OrderItemResponse.Projection, e =>
                e.BillId == MemberKey.BillId &&
                e.Bill.RestaurantId == RestaurantId &&
                e.Bill.BranchId == BranchKey.Id &&
                e.OrderId == order_id);
    }

    /// <summary>
    /// add item to order
    /// </summary>
    [HttpPost("{order_id}/items")]
    public async Task<ActionResult> CreateOrderItem(
        short order_id, OrderItemRequest body)
    {
        var availabilityResult = await orderingCalculator.CalculateStockAvailability(
            new()
            {
                [new(RestaurantId, body.menu_id)] = body.quantity
            },
            BranchKey);

        if (availabilityResult.IsFailed)
            return availabilityResult.Errors.ToActionResult();

        var result = await orderService.CreateItem(
            OrderItemResponse.Projection,
            new(MemberKey.BillId, order_id),
            new(RestaurantId, body.menu_id),
            body.quantity,
            body.note);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (key, response) = result.Value;

        return CreatedAtAction(
            nameof(GetOrderItem),
            new { order_id, item_id = key.Id },
            response);
    }

    /// <summary>
    /// update order's items
    /// </summary>
    [HttpPut("{order_id}/items")]
    public async Task<ActionResult> UpdateOrderItems(
        short order_id, IReadOnlyCollection<OrderItemRequest> body)
    {
        var availabilityResult = await orderingCalculator.CalculateStockAvailability(
            body.GroupBy(e => new MenuKey(RestaurantId, e.menu_id))
                .ToDictionary(
                    e => e.Key,
                    e => (short)e.Sum(i => i.quantity)),
            BranchKey);

        if (availabilityResult.IsFailed)
            return availabilityResult.Errors.ToActionResult();

        var command = body.Select(e =>
            new OrderItemCreateCommand(
                new(RestaurantId, e.menu_id),
                e.quantity,
                e.note));

        var result = await orderService.SetOrderItems(
            new(MemberKey.BillId, order_id),
            command);

        return NoContent();
    }

    /// <summary>
    /// get specific item
    /// </summary>
    [HttpGet("{order_id}/items/{item_id}")]
    public async Task<ActionResult<OrderItemResponse>> GetOrderItem(
        short order_id, short item_id)
    {
        var response = await orderService.GetItem(
            OrderItemResponse.Projection,
            new(MemberKey.BillId, order_id, item_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// update specific item
    /// </summary>
    [HttpPut("{order_id}/items/{item_id}")]
    public async Task<ActionResult<OrderItemResponse>> UpdateOrderItem(
        short order_id, short item_id, OrderItemUpdateRequest body)
    {
        var item = await orderService.GetItem(
            e => new { e.MenuId },
            new(MemberKey.BillId, order_id, item_id));

        if (item is null)
            return NotFound();

        var availabilityResult = await orderingCalculator.CalculateStockAvailability(
            new()
            {
                [new(RestaurantId, item.MenuId)] = body.quantity
            },
            BranchKey);

        if (availabilityResult.IsFailed)
            return availabilityResult.Errors.ToActionResult();

        var result = await orderService.UpdateItem(
            new(MemberKey.BillId, order_id, item_id),
            new(body.quantity, body.note));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// remove specific item
    /// </summary>
    [HttpDelete("{order_id}/items/{item_id}")]
    public async Task<ActionResult> DeleteOrderItem(short order_id, short item_id)
    {
        var result = await orderService.DeleteItem(
            new(MemberKey.BillId, order_id, item_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}