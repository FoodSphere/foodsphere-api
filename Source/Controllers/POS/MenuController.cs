using Microsoft.AspNetCore.Mvc;
using FoodSphere.Services;
using FoodSphere.Data;
using FoodSphere.Data.Models;

namespace FoodSphere.Controllers.Client;

public class MenuIngredientDTO
{
    public short ingredient_id { get; set; }
    public decimal amount { get; set; }

    public static MenuIngredientDTO FromModel(MenuIngredient model)
    {
        return new MenuIngredientDTO
        {
            ingredient_id = model.IngredientId,
            amount = model.Amount,
        };
    }
}

public class MenuRequest
{
    public List<MenuIngredientDTO> ingredients { get; set; } = [];

    public required string name { get; set; }
    public string? display_name { get; set; }
    public string? description { get; set; }
    public string? image_url { get; set; }
    public int price { get; set; }
}

public class MenuResponse
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    public List<MenuIngredientDTO> ingredients { get; set; } = [];

    public required string name { get; set; }
    public string? display_name { get; set; }
    public string? description { get; set; }
    public string? image_url { get; set; }
    public int price { get; set; }
    public MenuStatus status { get; set; }

    public static MenuResponse FromModel(Menu model)
    {
        return new MenuResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            ingredients = [.. model.MenuIngredients.Select(MenuIngredientDTO.FromModel)],
            name = model.Name,
            display_name = model.DisplayName,
            description = model.Description,
            image_url = model.ImageUrl,
            price = model.Price,
            status = model.Status,
        };
    }
}

[Route("client/restaurants/{restaurant_id}/menus")]
public class MenuController(
    ILogger<MenuController> logger,
    MenuService menuService
) : ClientController
{
    readonly ILogger<MenuController> _logger = logger;
    readonly MenuService _menuService = menuService;

    [HttpPost]
    public async Task<ActionResult<MenuResponse>> CreateMenu(Guid restaurant_id, MenuRequest body)
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
            MenuResponse.FromModel(menu)
        );
    }

    [HttpGet("{menu_id}")]
    public async Task<ActionResult<MenuResponse>> GetMenu(Guid restaurant_id, short menu_id)
    {
        var menu = await _menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        return MenuResponse.FromModel(menu);
    }

    [HttpPut("{menu_id}")]
    public async Task<ActionResult> UpdateMenu(Guid restaurant_id, short menu_id, MenuRequest body)
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
    public async Task<ActionResult> UpdateMenuIngredient(Guid restaurant_id, short menu_id, MenuIngredientDTO body)
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