using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FoodSphere.Services;
using FoodSphere.Data.Models;
using FoodSphere.Data;
using FoodSphere.Data.DTOs;

namespace FoodSphere.Controllers.Client;

public class StockDTO
{
    public short ingredient_id { get; set; }
    public decimal amount { get; set; }

    public static StockDTO FromModel(Stock model)
    {
        return new StockDTO
        {
            ingredient_id = model.IngredientId,
            amount = model.Amount,
        };
    }
}

[Route("client/restaurants/{restaurant_id}/branches/{branch_id}/stocks")]
public class StockController(
    ILogger<StockController> logger,
    BranchService branchService
) : ClientController
{
    readonly ILogger<StockController> _logger = logger;
    readonly BranchService _branchService = branchService;

    [HttpGet]
    public async Task<ActionResult<List<StockDTO>>> ListStocks(Guid restaurant_id, short branch_id)
    {
        var branch = await _branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        return branch.IngredientStocks
            .Select(StockDTO.FromModel)
            .ToList();
    }

    [HttpPost]
    public async Task<ActionResult> SetStock(Guid restaurant_id, short branch_id, StockDTO body)
    {
        var branch = await _branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        await _branchService.SetStock(branch, body.ingredient_id, body.amount);
        await _branchService.Save();

        return NoContent();
    }

    [HttpGet("{ingredient_id}")]
    public async Task<ActionResult<StockDTO>> GetStocks(Guid restaurant_id, short branch_id, short ingredient_id)
    {
        var stock = await _branchService.GetStock(restaurant_id, branch_id, ingredient_id);

        if (stock is null)
        {
            return NotFound();
        }

        return StockDTO.FromModel(stock);
    }

    [HttpDelete("{ingredient_id}")]
    public async Task<ActionResult> DeleteStock(Guid restaurant_id, short branch_id, short ingredient_id)
    {
        var branch = await _branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        var stock = await _branchService.GetStock(branch, ingredient_id);

        if (stock is null)
        {
            return NotFound();
        }

        await _branchService.DeleteStock(stock);
        await _branchService.Save();

        return NoContent();
    }
}