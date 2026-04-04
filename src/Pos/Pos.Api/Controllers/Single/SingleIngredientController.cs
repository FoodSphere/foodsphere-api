namespace FoodSphere.Pos.Api.Controller;

[Route("s/restaurants/{restaurant_id}/ingredients")]
public class SingleIngredientController(
    ILogger<SingleIngredientController> logger,
    AccessControlService accessControl,
    PersistenceService persistenceService,
    IngredientServiceBase ingredientService,
    StockServiceBase stockService
) : PosControllerBase
{
    /// <summary>
    /// list ingredients
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<SingleIngredientResponse>>> ListIngredients(
        Guid restaurant_id,
        [FromQuery] bool? is_deleted = false)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.READ, PERMISSION.Stock.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        Expression<Func<StockTransaction, bool>> predicate = e =>
            e.RestaurantId == restaurant_id &&
            e.BranchId == 1;

        if (is_deleted is not null)
            predicate = predicate.And(e =>
                e.Ingredient.DeleteTime != null == is_deleted.Value);

        return await stockService.ListLatestTransactionOfIngredients(
            SingleIngredientResponse.Projection, predicate);
    }

    /// <summary>
    /// create ingredient
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SingleIngredientResponse>> CreateIngredient(
        Guid restaurant_id, SingleIngredientRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.CREATE, PERMISSION.Stock.CREATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var tagKeys = body.tags.Select(e =>
            new TagKey(restaurant_id, e.tag_id));

        await using var transaction = await persistenceService.BeginTransaction();

        var ingredientResult = await ingredientService.CreateIngredient(
            e => true, new(
                RestaurantKey: new(restaurant_id),
                Name: body.name,
                Tags: tagKeys,
                Unit: body.unit,
                Description: body.description,
                Status: body.status));

        if (ingredientResult.IsFailed)
            return ingredientResult.Errors.ToActionResult();

        var (ingredientKey, _) = ingredientResult.Value;

        var stockResult = await stockService.CreateTransaction(
            SingleIngredientResponse.Projection, new(
                new(restaurant_id, 1),
                ingredientKey,
                body.stock,
                "initial stock"));

        if (stockResult.IsFailed)
            return stockResult.Errors.ToActionResult();

        await transaction.CommitAsync();

        var (_, response) = stockResult.Value;

        return CreatedAtAction(
            nameof(GetIngredient),
            new { restaurant_id, ingredient_id = ingredientKey.Id },
            response);
    }

    /// <summary>
    /// get ingredient
    /// </summary>
    [HttpGet("{ingredient_id}")]
    public async Task<ActionResult<SingleIngredientResponse>> GetIngredient(
        Guid restaurant_id, short ingredient_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.READ, PERMISSION.Stock.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var response = await stockService.GetIngredientLatestTransaction(
            SingleIngredientResponse.Projection,
            new(restaurant_id, 1), new(restaurant_id, ingredient_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// update ingredient
    /// </summary>
    [HttpPut("{ingredient_id}")]
    public async Task<ActionResult> UpdateIngredient(
        Guid restaurant_id, short ingredient_id, SingleIngredientRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.UPDATE, PERMISSION.Stock.CREATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var tagKeys = body.tags.Select(e =>
            new TagKey(restaurant_id, e.tag_id));

        await using var transaction = await persistenceService.BeginTransaction();

        var ingredientKey = new IngredientKey(restaurant_id, ingredient_id);

        var ingredientResult = await ingredientService.UpdateIngredient(
            ingredientKey, new(
                Name: body.name,
                Tags: tagKeys,
                Unit: body.unit,
                Description: body.description,
                Status: body.status));

        if (ingredientResult.IsFailed)
            return ingredientResult.Errors.ToActionResult();

        var stockResult = await stockService.RebalanceTransaction(
            SingleIngredientResponse.Projection, new(
                new(restaurant_id, 1),
                ingredientKey,
                body.stock,
                "rebalance via ingredient's stock update"));

        if (stockResult.IsFailed)
            return stockResult.Errors.ToActionResult();

        await transaction.CommitAsync();

        return NoContent();
    }
}