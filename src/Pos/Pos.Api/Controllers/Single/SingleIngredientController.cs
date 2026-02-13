namespace FoodSphere.Pos.Api.Controller;

[Route("s/restaurants/{restaurant_id}/ingredients")]
public class SingleIngredientController(
    ILogger<SingleIngredientController> logger,
    IngredientService ingredientService,
    BranchService branchService
) : PosControllerBase
{
    /// <summary>
    /// list ingredients
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<SingleIngredientResponse[]>> ListIngredients(Guid restaurant_id)
    {
        var stocks = await branchService.ListDefaultStocks(restaurant_id);

        if (stocks is null)
        {
            return NotFound();
        }

        return stocks
            .Select(SingleIngredientResponse.FromModel)
            .ToArray();
    }

    /// <summary>
    /// create ingredient
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SingleIngredientResponse>> CreateIngredient(Guid restaurant_id, SingleIngredientRequest body)
    {
        var branch = await branchService.GetDefaultBranch(restaurant_id);

        if (branch is null)
        {
            return NotFound();
        }

        var ingredient = await ingredientService.CreateIngredient(
            restaurantId: restaurant_id,
            name: body.name,
            description: body.description,
            unit: body.unit
        );

        var stock = await branchService.SetStock(branch, ingredient.Id, body.stock);

        await branchService.SaveChanges();

        return CreatedAtAction(
            nameof(GetIngredient),
            new { restaurant_id, ingredient_id = ingredient.Id },
            SingleIngredientResponse.FromModel(stock)
        );
    }

    /// <summary>
    /// get ingredient
    /// </summary>
    [HttpGet("{ingredient_id}")]
    public async Task<ActionResult<SingleIngredientResponse>> GetIngredient(Guid restaurant_id, short ingredient_id)
    {
        var stock = await branchService.GetDefaultStock(restaurant_id, ingredient_id);

        if (stock is null)
        {
            return NotFound();
        }

        return SingleIngredientResponse.FromModel(stock);
    }

    /// <summary>
    /// update ingredient
    /// </summary>
    [HttpPut("{ingredient_id}")]
    public async Task<ActionResult<SingleIngredientResponse>> UpdateIngredient(Guid restaurant_id, short ingredient_id, SingleIngredientRequest body)
    {
        var stock = await branchService.GetDefaultStock(restaurant_id, ingredient_id);

        if (stock is null)
        {
            return NotFound();
        }

        stock.Ingredient.Name = body.name;
        stock.Ingredient.Description = body.description;
        stock.Ingredient.Unit = body.unit;
        stock.Amount = body.stock;

        await branchService.SaveChanges();

        return SingleIngredientResponse.FromModel(stock);
    }
}