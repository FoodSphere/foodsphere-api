namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/branches/{branch_id}/stock")]
public class StockController(
    ILogger<StockController> logger,
    StockServiceBase stockService,
    AccessControlService accessControl
) : PosControllerBase
{
    /// <summary>
    /// list stock's transactions
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<ICollection<StockTransactionResponse>>> ListTransactions(
        Guid restaurant_id, short branch_id,
        [FromQuery] IReadOnlyCollection<short> ingredient_id,
        [FromQuery] IReadOnlyCollection<Guid> bill_id,
        [FromQuery] DateTime? from_time,
        [FromQuery] DateTime? to_time,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 10000)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Stock.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        Expression<Func<StockTransaction, bool>> predicate = e =>
            e.RestaurantId == restaurant_id &&
            e.BranchId == branch_id;

        if (ingredient_id.Count > 0)
            predicate = predicate.And(e =>
                ingredient_id.Contains(e.IngredientId));

        if (bill_id.Count > 0)
            predicate = predicate.And(e =>
                e.BillId != null &&
                bill_id.Contains(e.BillId.Value));

        if (from_time is not null)
            predicate = predicate.And(e => e.CreateTime >= from_time.Value);

        if (to_time is not null)
            predicate = predicate.And(e => e.CreateTime <= to_time.Value);

        return await stockService.ListTransactions(
            StockTransactionResponse.Projection, predicate,
            new(offset, limit));
    }

    /// <summary>
    /// create stock transaction
    /// </summary>
    [HttpPost("transactions")]
    public async Task<ActionResult<StockTransactionResponse>> CreateTransaction(
        Guid restaurant_id, short branch_id, StockTransactionRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Stock.CREATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await stockService.CreateTransaction(
            StockTransactionResponse.Projection, new(
                new(restaurant_id, branch_id),
                new(restaurant_id, body.ingredient_id),
                body.amount,
                body.note));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (key, response) = result.Value;

        return CreatedAtAction(
            nameof(GetStock),
            new { restaurant_id, branch_id, key.Id },
            response);
    }

    /// <summary>
    /// get transaction
    /// </summary>
    [HttpGet("transactions/{transaction_id}")]
    public async Task<ActionResult<StockTransactionResponse>> GetStock(
        Guid restaurant_id, short branch_id, Guid transaction_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Stock.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var response = await stockService.GetTransaction(
            StockTransactionResponse.Projection,
            new(restaurant_id, branch_id, transaction_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// create stock transaction (adjust stock level)
    /// </summary>
    [HttpPut("rebalance")]
    public async Task<ActionResult<StockTransactionResponse>> SetTransaction(
        Guid restaurant_id, short branch_id, StockTransactionRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Stock.CREATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await stockService.CreateTransaction(
            StockTransactionResponse.Projection, new(
                new(restaurant_id, branch_id),
                new(restaurant_id, body.ingredient_id),
                body.amount,
                body.note));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (key, response) = result.Value;

        return CreatedAtAction(
            nameof(GetStock),
            new { restaurant_id, branch_id, key.Id },
            response);
    }
}