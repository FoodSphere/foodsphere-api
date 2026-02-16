namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/branches/{branch_id}/stocks")]
public class StockController(
    ILogger<StockController> logger,
    BranchService branchService
) : PosControllerBase
{
    /// <summary>
    /// list stocks
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<StockDto>>> ListStocks(Guid restaurant_id, short branch_id)
    {
        var responses = await branchService.QueryStocks()
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == branch_id)
            .Select(StockDto.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// upsert stock
    /// </summary>
    [HttpPut]
    public async Task<ActionResult> UpsertStock(Guid restaurant_id, short branch_id, StockDto body)
    {
        var stock = await branchService.GetStock(restaurant_id, branch_id, body.ingredient_id);

        if (stock is null)
        {
            var branch = branchService.GetBranchStub(restaurant_id, branch_id);

            stock = await branchService.CreateStock(branch, body.ingredient_id ,body.amount);

            await branchService.SaveChanges();

            var response = await branchService.GetStock(
                restaurant_id, branch_id, body.ingredient_id,
                StockDto.Projection);

            return CreatedAtAction(
                nameof(GetStock),
                new { restaurant_id, branch_id, ingredient_id = body.ingredient_id },
                response);
        }

        stock.Amount = body.amount;

        await branchService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// get stock
    /// </summary>
    [HttpGet("{ingredient_id}")]
    public async Task<ActionResult<StockDto>> GetStock(Guid restaurant_id, short branch_id, short ingredient_id)
    {
        var response = await branchService.GetStock(
            restaurant_id, branch_id, ingredient_id,
            StockDto.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// delete stock
    /// </summary>
    [HttpDelete("{ingredient_id}")]
    public async Task<ActionResult> DeleteStock(Guid restaurant_id, short branch_id, short ingredient_id)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        var stock = await branchService.GetStock(branch, ingredient_id);

        if (stock is null)
        {
            return NotFound();
        }

        await branchService.DeleteStock(stock);
        await branchService.SaveChanges();

        return NoContent();
    }
}