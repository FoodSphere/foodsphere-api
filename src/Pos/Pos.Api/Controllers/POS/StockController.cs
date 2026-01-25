using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.Pos.Api.Controllers;

[Route("restaurants/{restaurant_id}/branches/{branch_id}/stocks")]
public class StockController(
    ILogger<StockController> logger,
    BranchService branchService
) : PosControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StockDto>>> ListStocks(Guid restaurant_id, short branch_id)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        return branch.IngredientStocks
            .Select(StockDto.FromModel)
            .ToList();
    }

    [HttpPost]
    public async Task<ActionResult> SetStock(Guid restaurant_id, short branch_id, StockDto body)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        await branchService.SetStock(branch, body.ingredient_id, body.amount);
        await branchService.SaveAsync();

        return NoContent();
    }

    [HttpGet("{ingredient_id}")]
    public async Task<ActionResult<StockDto>> GetStocks(Guid restaurant_id, short branch_id, short ingredient_id)
    {
        var stock = await branchService.GetStock(restaurant_id, branch_id, ingredient_id);

        if (stock is null)
        {
            return NotFound();
        }

        return StockDto.FromModel(stock);
    }

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
        await branchService.SaveAsync();

        return NoContent();
    }
}