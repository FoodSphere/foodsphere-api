namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/report")]
public class ReportController(
    ILogger<ReportController> logger,
    PaymentService paymentService,
    ReportCalculator reportCalculator,
    AccessControlService accessControl
) : MasterControllerBase
{
    /// <summary>
    /// list payments
    /// </summary>
    [HttpGet("payments")]
    public async Task<ActionResult<ICollection<PaymentResponse>>> ListPayments(
        Guid restaurant_id,
        [FromQuery] IReadOnlyCollection<Guid> bill_id,
        [FromQuery] IReadOnlyCollection<PaymentStatus> payment_status,
        [FromQuery] DateTime? from_time,
        [FromQuery] DateTime? to_time)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Dashboard.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        Expression<Func<Payment, bool>> predicate = e =>
            e.Bill.RestaurantId == restaurant_id;

        if (bill_id.Count != 0)
            predicate = predicate.And(e => bill_id.Contains(e.BillId));

        if (payment_status.Count != 0)
            predicate = predicate.And(e => payment_status.Contains(e.Status));

        if (from_time is not null)
            predicate = predicate.And(e => e.CreateTime >= from_time.Value);

        if (to_time is not null)
            predicate = predicate.And(e => e.CreateTime <= to_time.Value);

        return await paymentService.ListPayments(
            PaymentResponse.Projection, predicate);
    }

    /// <summary>
    /// get revenue
    /// </summary>
    [HttpGet("revenue")]
    public async Task<ActionResult<RevenueResponse>> GetRevenue(
        Guid restaurant_id,
        [FromQuery] DateTime? from_time,
        [FromQuery] DateTime? to_time)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Dashboard.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var revenue = await reportCalculator.GetRevenue(
            new(restaurant_id),
            from_time, to_time);

        return new RevenueResponse()
        {
            revenue = revenue,
        };
    }

    /// <summary>
    /// count bills
    /// </summary>
    [HttpGet("bill")]
    public async Task<ActionResult<BillCountResponse>> CountBill(
        Guid restaurant_id,
        [FromQuery] IReadOnlyCollection<BillStatus> bill_status,
        [FromQuery] DateTime? from_time,
        [FromQuery] DateTime? to_time)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Dashboard.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        Expression<Func<Bill, bool>> predicate = e =>
            e.RestaurantId == restaurant_id;

        if (bill_status.Count != 0)
            predicate = predicate.And(e => bill_status.Contains(e.Status));

        if (from_time is not null)
            predicate = predicate.And(e => e.CreateTime >= from_time.Value);

        if (to_time is not null)
            predicate = predicate.And(e => e.CreateTime <= to_time.Value);

        var billCount = await reportCalculator.CountBill(predicate);

        return new BillCountResponse()
        {
            bill_count = billCount,
        };
    }

    /// <summary>
    /// count menu sold
    /// </summary>
    [HttpGet("menu-sold/count")]
    public async Task<ActionResult<MenuSoldResponse>> GetMenuSold(
        Guid restaurant_id,
        [FromQuery] DateTime? from_time,
        [FromQuery] DateTime? to_time)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Dashboard.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        Expression<Func<OrderItem, bool>> predicate = e =>
            e.RestaurantId == restaurant_id && (
                e.Order.Status == OrderStatus.Cooking ||
                e.Order.Status == OrderStatus.Served);

        if (from_time is not null)
            predicate = predicate.And(e => e.CreateTime >= from_time.Value);

        if (to_time is not null)
            predicate = predicate.And(e => e.CreateTime <= to_time.Value);

        var sold_count = await reportCalculator.CountMenuSold(predicate);

        return new MenuSoldResponse()
        {
            menu_sold = sold_count,
        };
    }

    /// <summary>
    /// menu sold data
    /// </summary>
    [HttpGet("menu-sold")]
    public async Task<ActionResult<ICollection<MenuSoldItemResponse>>> ListMenuSold(
        Guid restaurant_id,
        [FromQuery] DateTime? from_time,
        [FromQuery] DateTime? to_time)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Dashboard.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        Expression<Func<OrderItem, bool>> predicate = e =>
            e.Bill.RestaurantId == restaurant_id && (
                e.Order.Status == OrderStatus.Cooking ||
                e.Order.Status == OrderStatus.Served);

        if (from_time is not null)
            predicate = predicate.And(e => e.CreateTime >= from_time.Value);

        if (to_time is not null)
            predicate = predicate.And(e => e.CreateTime <= to_time.Value);

        return await reportCalculator.ListMenuSold(predicate);
    }

    /// <summary>
    /// stock usage
    /// </summary>
    [HttpGet("stock-usage")]
    public async Task<ActionResult<ICollection<StockUsageResponse>>> ListStockUsage(
        Guid restaurant_id,
        [FromQuery] DateTime? from_time,
        [FromQuery] DateTime? to_time)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Dashboard.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        return await reportCalculator.ListStockUsage(
            new(restaurant_id), from_time, to_time);
    }
}