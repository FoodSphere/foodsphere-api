using Microsoft.AspNetCore.Mvc;
using FoodSphere.Services;
using FoodSphere.Data;
using FoodSphere.Data.Models;

namespace FoodSphere.Controllers.Resource;

public class ResourceMenuIngredientDTO
{
    public short ingredient_id { get; set; }
    public decimal amount { get; set; }

    public static ResourceMenuIngredientDTO FromModel(MenuIngredient model)
    {
        return new ResourceMenuIngredientDTO
        {
            ingredient_id = model.IngredientId,
            amount = model.Amount,
        };
    }
}

public class ResourceMenuRequest
{
    public List<ResourceMenuIngredientDTO> ingredients { get; set; } = [];

    public required string name { get; set; }
    public string? display_name { get; set; }
    public string? description { get; set; }
    public string? image_url { get; set; }
    public short price { get; set; }
}

public class ResourceMenuResponse
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    public List<ResourceMenuIngredientDTO> ingredients { get; set; } = [];

    public required string name { get; set; }
    public string? display_name { get; set; }
    public string? description { get; set; }
    public string? image_url { get; set; }
    public int price { get; set; }
    public MenuStatus status { get; set; }

    public static ResourceMenuResponse FromModel(Menu model)
    {
        return new ResourceMenuResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            ingredients = [.. model.MenuIngredients.Select(ResourceMenuIngredientDTO.FromModel)],
            name = model.Name,
            display_name = model.DisplayName,
            description = model.Description,
            image_url = model.ImageUrl,
            price = model.Price,
            status = model.Status,
        };
    }
}

[Route("resource/restaurants/{restaurant_id}/menus")]
public class ResourceMenuController(
    ILogger<ResourceMenuController> logger,
    MenuService menuService
) : AdminController
{
    readonly ILogger<ResourceMenuController> _logger = logger;
    readonly MenuService _menuService = menuService;

    [HttpPost]
    public async Task<ActionResult<ResourceMenuResponse>> CreateMenu(Guid restaurant_id, ResourceMenuRequest body)
    {
        var menu = await _menuService.CreateMenu(
            restaurantId: restaurant_id,
            name: body.name,
            price: body.price,
            displayName: body.display_name,
            description: body.description,
            imageUrl: body.image_url
        );

        foreach (var ingredient in body.ingredients)
        {
            await _menuService.UpdateIngredient(restaurant_id, menu.Id, ingredient.ingredient_id, ingredient.amount);
        }

        await _menuService.Save();

        return CreatedAtAction(
            nameof(GetMenu),
            new { restaurant_id, menu_id = menu.Id },
            ResourceMenuResponse.FromModel(menu)
        );
    }

    [HttpGet("{menu_id}")]
    public async Task<ActionResult<ResourceMenuResponse>> GetMenu(Guid restaurant_id, short menu_id)
    {
        var menu = await _menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        return ResourceMenuResponse.FromModel(menu);
    }

    [HttpPut("{menu_id}")]
    public async Task<ActionResult> UpdateMenu(Guid restaurant_id, short menu_id, ResourceMenuRequest body)
    {
        var menu = await _menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        menu.Name = body.name;
        menu.Price = body.price;
        menu.DisplayName = body?.display_name;
        menu.Description = body?.description;
        menu.ImageUrl = body?.image_url;

        await _menuService.Save();

        return NoContent();
    }

    [HttpPost("{menu_id}/ingredients")]
    public async Task<ActionResult> UpdateMenuIngredient(Guid restaurant_id, short menu_id, ResourceMenuIngredientDTO body)
    {
        var menu = await _menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        await _menuService.UpdateIngredient(restaurant_id, menu.Id, body.ingredient_id, body.amount);
        await _menuService.Save();

        return NoContent();
    }

    [HttpDelete("{menu_id}")]
    public async Task<ActionResult> DeleteMenu(Guid restaurant_id, short menu_id)
    {
        var menu = await _menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        await _menuService.DeleteMenu(menu);
        await _menuService.Save();

        return NoContent();
    }
}