namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/ingredients")]
public class IngredientController(
    ILogger<IngredientController> logger,
    IngredientService ingredientService
) : PosControllerBase
{
    /// <summary>
    /// list ingredients
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<IngredientResponse>>> ListIngredients(Guid restaurant_id)
    {
        var ingredients = await ingredientService.ListIngredients(restaurant_id);

        return ingredients
            .Select(IngredientResponse.FromModel)
            .ToList();
    }

    /// <summary>
    /// create ingredient
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<IngredientResponse>> CreateIngredient(Guid restaurant_id, IngredientRequest body)
    {
        var ingredient = await ingredientService.CreateIngredient(
            restaurantId: restaurant_id,
            name: body.name,
            description: body.description,
            unit: body.unit
        );

        await ingredientService.SaveChanges();

        return CreatedAtAction(
            nameof(GetIngredient),
            new { restaurant_id, ingredient_id = ingredient.Id },
            IngredientResponse.FromModel(ingredient)
        );
    }

    /// <summary>
    /// get ingredient
    /// </summary>
    [HttpGet("{ingredient_id}")]
    public async Task<ActionResult<IngredientResponse>> GetIngredient(Guid restaurant_id, short ingredient_id)
    {
        var ingredient = await ingredientService.GetIngredient(restaurant_id, ingredient_id);

        if (ingredient is null)
        {
            return NotFound();
        }

        return IngredientResponse.FromModel(ingredient);
    }

    /// <summary>
    /// update ingredient
    /// </summary>
    [HttpPut("{ingredient_id}")]
    public async Task<ActionResult> UpdateIngredient(Guid restaurant_id, short ingredient_id, IngredientRequest body)
    {
        var ingredient = await ingredientService.GetIngredient(restaurant_id, ingredient_id);

        if (ingredient is null)
        {
            return NotFound();
        }

        ingredient.Name = body.name;
        ingredient.Description = body?.description;
        ingredient.Unit = body?.unit;

        await ingredientService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// delete ingredient
    /// </summary>
    [HttpDelete("{ingredient_id}")]
    public async Task<ActionResult> DeleteIngredient(Guid restaurant_id, short ingredient_id)
    {
        var ingredient = await ingredientService.GetIngredient(restaurant_id, ingredient_id);

        if (ingredient is null)
        {
            return NotFound();
        }

        await ingredientService.DeleteIngredient(ingredient);
        await ingredientService.SaveChanges();

        return NoContent();
    }
}
