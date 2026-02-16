namespace FoodSphere.Pos.Api.Controller;

[Route("s/restaurants/{restaurant_id}/ingredients")]
public class SingleIngredientController(
    ILogger<SingleIngredientController> logger,
    AccessControlService accessControl,
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
        var hasPermission = await accessControl.Validate(
            User, restaurant_id,
            PERMISSION.Ingredient.READ
        );

        if (!hasPermission)
        {
            return Forbid();
        }

        var responses = await branchService
            .QueryStocks()
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1)
            .Select(SingleIngredientResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create ingredient
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SingleIngredientResponse>> CreateIngredient(
        Guid restaurant_id, SingleIngredientRequest body)
    {
        var hasPermission = await accessControl.Validate(
            User, restaurant_id,
            PERMISSION.Ingredient.CREATE
        );

        if (!hasPermission)
        {
            return Forbid();
        }

        var branch = await branchService.GetBranch(restaurant_id, 1);

        if (branch is null)
        {
            return NotFound();
        }

        var ingredient = await ingredientService.CreateIngredient(
            restaurantId: restaurant_id,
            name: body.name,
            unit: body.unit,
            description: body.description
        );

        foreach (var tag in body.tags)
        {
            await ingredientService.AssignTag(ingredient, tag.tag_id);
        }

        var stock = await branchService.SetStock(branch, ingredient.Id, body.stock);

        await branchService.SaveChanges();

        var response = await branchService.GetStock(
            restaurant_id, 1, ingredient.Id,
            SingleIngredientResponse.Projection);

        return CreatedAtAction(
            nameof(GetIngredient),
            new { restaurant_id, ingredient_id = ingredient.Id },
            response);
    }

    /// <summary>
    /// get ingredient
    /// </summary>
    [HttpGet("{ingredient_id}")]
    public async Task<ActionResult<SingleIngredientResponse>> GetIngredient(Guid restaurant_id, short ingredient_id)
    {
        var response = await branchService.GetStock(
            restaurant_id, 1, ingredient_id,
            SingleIngredientResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// update ingredient
    /// </summary>
    [HttpPut("{ingredient_id}")]
    public async Task<ActionResult> UpdateIngredient(
        Guid restaurant_id, short ingredient_id,
        SingleIngredientRequest body)
    {
        var hasPermission = await accessControl.Validate(
            User, restaurant_id,
            PERMISSION.Ingredient.UPDATE
        );

        if (!hasPermission)
        {
            return Forbid();
        }

        var stock = await branchService.GetStock(restaurant_id, 1, ingredient_id);

        if (stock is null)
        {
            return NotFound();
        }

        stock.Ingredient.Name = body.name;
        stock.Ingredient.Description = body.description;
        stock.Ingredient.Unit = body.unit;
        stock.Amount = body.stock;

        await ingredientService
            .IngredientTagQuery(restaurant_id, ingredient_id)
            .ExecuteDeleteAsync();

        foreach (var tag in body.tags)
        {
            await ingredientService.AssignTag(stock.Ingredient, tag.tag_id);
        }

        await branchService.SaveChanges();

        return NoContent();
    }
}