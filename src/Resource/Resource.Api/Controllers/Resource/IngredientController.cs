// namespace FoodSphere.Resource.Api.Controller;

// [Route("restaurants/{restaurant_id}/ingredients")]
// public class IngredientController(
//     ILogger<IngredientController> logger,
//     IngredientService ingredientService
// ) : ResourceControllerBase
// {
//     [HttpGet]
//     public async Task<ActionResult<List<IngredientResponse>>> ListIngredients(Guid restaurant_id)
//     {
//         var ingredients = await ingredientService.ListIngredients(restaurant_id);

//         return ingredients.Select(IngredientResponse.FromModel).ToList();
//     }

//     [HttpPost]
//     public async Task<ActionResult<IngredientResponse>> CreateIngredient(Guid restaurant_id, IngredientRequest body)
//     {
//         var ingredient = await ingredientService.CreateIngredient(
//             restaurantId: restaurant_id,
//             name: body.name,
//             description: body.description,
//             imageUrl: body.image_url,
//             unit: body.unit
//         );

//         await ingredientService.SaveChanges();

//         return CreatedAtAction(
//             nameof(GetIngredient),
//             new { restaurant_id, ingredient_id = ingredient.Id },
//             IngredientResponse.FromModel(ingredient)
//         );
//     }

//     [HttpGet("{ingredient_id}")]
//     public async Task<ActionResult<IngredientResponse>> GetIngredient(Guid restaurant_id, short ingredient_id)
//     {
//         var ingredient = await ingredientService.GetIngredient(restaurant_id, ingredient_id);

//         if (ingredient is null)
//         {
//             return NotFound();
//         }

//         return IngredientResponse.FromModel(ingredient);
//     }

//     [HttpPut("{ingredient_id}")]
//     public async Task<ActionResult> UpdateIngredient(Guid restaurant_id, short ingredient_id, IngredientRequest body)
//     {
//         var ingredient = await ingredientService.GetIngredient(restaurant_id, ingredient_id);

//         if (ingredient is null)
//         {
//             return NotFound();
//         }

//         ingredient.Name = body.name;
//         ingredient.Description = body?.description;
//         ingredient.ImageUrl = body?.image_url;
//         ingredient.Unit = body?.unit;

//         await ingredientService.SaveChanges();

//         return NoContent();
//     }

//     [HttpDelete("{ingredient_id}")]
//     public async Task<ActionResult> DeleteIngredient(Guid restaurant_id, short ingredient_id)
//     {
//         var ingredient = await ingredientService.GetIngredient(restaurant_id, ingredient_id);

//         if (ingredient is null)
//         {
//             return NotFound();
//         }

//         await ingredientService.DeleteIngredient(ingredient);
//         await ingredientService.SaveChanges();

//         return NoContent();
//     }
// }
