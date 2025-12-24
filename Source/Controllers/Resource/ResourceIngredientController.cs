using Microsoft.AspNetCore.Mvc;
using FoodSphere.Services;
using FoodSphere.Data;
using FoodSphere.Data.Models;

namespace FoodSphere.Controllers.Resource;

public class ResourceIngredientRequest
{
    public required string name { get; set; }
    public string? description { get; set; }
    public string? image_url { get; set; }
    public string? unit { get; set; }
}

public class ResourceIngredientResponse
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    public required string name { get; set; }
    public string? description { get; set; }
    public string? image_url { get; set; }
    public string? unit { get; set; }
    public IngredientStatus status { get; set; }

    public static ResourceIngredientResponse FromModel(Ingredient model)
    {
        return new ResourceIngredientResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            name = model.Name,
            description = model.Description,
            image_url = model.ImageUrl,
            unit = model.Unit,
            status = model.Status,
        };
    }
}

[Route("resource/restaurants/{restaurant_id}/ingredients")]
public class ResourceIngredientController(
    ILogger<ResourceIngredientController> logger,
    MenuService menuService
) : AdminController
{
    readonly ILogger<ResourceIngredientController> _logger = logger;
    readonly MenuService _menuService = menuService;

    [HttpPost]
    public async Task<ActionResult<ResourceIngredientResponse>> CreateIngredient(Guid restaurant_id, ResourceIngredientRequest body)
    {
        var ingredient = await _menuService.CreateIngredient(
            restaurantId: restaurant_id,
            name: body.name,
            description: body.description,
            imageUrl: body.image_url,
            unit: body.unit
        );

        await _menuService.Save();

        return CreatedAtAction(
            nameof(GetIngredient),
            new { restaurant_id, ingredient_id = ingredient.Id },
            ResourceIngredientResponse.FromModel(ingredient)
        );
    }

    [HttpGet("{ingredient_id}")]
    public async Task<ActionResult<ResourceIngredientResponse>> GetIngredient(Guid restaurant_id, short ingredient_id)
    {
        var ingredient = await _menuService.GetIngredient(restaurant_id, ingredient_id);

        if (ingredient is null)
        {
            return NotFound();
        }

        return ResourceIngredientResponse.FromModel(ingredient);
    }

    [HttpPut("{ingredient_id}")]
    public async Task<ActionResult> UpdateIngredient(Guid restaurant_id, short ingredient_id, ResourceIngredientRequest body)
    {
        var ingredient = await _menuService.GetIngredient(restaurant_id, ingredient_id);

        if (ingredient is null)
        {
            return NotFound();
        }

        ingredient.Name = body.name;
        ingredient.Description = body?.description;
        ingredient.ImageUrl = body?.image_url;
        ingredient.Unit = body?.unit;

        await _menuService.Save();

        return NoContent();
    }

    [HttpDelete("{ingredient_id}")]
    public async Task<ActionResult> DeleteIngredient(Guid restaurant_id, short ingredient_id)
    {
        var ingredient = await _menuService.GetIngredient(restaurant_id, ingredient_id);

        if (ingredient is null)
        {
            return NotFound();
        }

        await _menuService.DeleteIngredient(ingredient);
        await _menuService.Save();

        return NoContent();
    }
}
