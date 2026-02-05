namespace FoodSphere.Pos.Api.Controllers;

[Route("restaurants/{restaurant_id}/menus")]
public class MenuController(
    ILogger<MenuController> logger,
    CheckPermissionService checkPermissionService,
    MenuService menuService
) : PosControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MenuResponse>>> ListMenus(Guid restaurant_id)
    {
        var menus = await menuService.ListMenus(restaurant_id);

        return menus
            .Select(MenuResponse.FromModel)
            .ToList();
    }

    [HttpPost]
    public async Task<ActionResult<MenuResponse>> CreateMenu(Guid restaurant_id, MenuRequest body)
    {
        var hasPermission = await checkPermissionService.CheckPermission(
            User, restaurant_id,
            PERMISSION.Menu.CREATE
        );

        if (!hasPermission)
        {
            return Forbid();
        }

        var menu = await menuService.CreateMenu(
            restaurantId: restaurant_id,
            name: body.name,
            price: body.price,
            displayName: body.display_name,
            description: body.description,
            imageUrl: body.image_url
        );

        foreach (var ingredient in body.ingredients)
        {
            await menuService.UpdateIngredient(restaurant_id, menu.Id, ingredient.ingredient_id, ingredient.amount);
        }

        await menuService.SaveAsync();

        return CreatedAtAction(
            nameof(GetMenu),
            new { restaurant_id, menu_id = menu.Id },
            MenuResponse.FromModel(menu)
        );
    }

    [HttpGet("{menu_id}")]
    public async Task<ActionResult<MenuResponse>> GetMenu(Guid restaurant_id, short menu_id)
    {
        var menu = await menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        return MenuResponse.FromModel(menu);
    }

    [HttpPut("{menu_id}")]
    public async Task<ActionResult> UpdateMenu(Guid restaurant_id, short menu_id, MenuRequest body)
    {
        var hasPermission = await checkPermissionService.CheckPermission(
            User, restaurant_id,
            PERMISSION.Menu.UPDATE
        );

        if (!hasPermission)
        {
            return Forbid();
        }

        var menu = await menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        menu.Name = body.name;
        menu.Price = body.price;
        menu.DisplayName = body?.display_name;
        menu.Description = body?.description;
        menu.ImageUrl = body?.image_url;

        await menuService.SaveAsync();

        return NoContent();
    }

    [HttpPost("{menu_id}/ingredients")]
    public async Task<ActionResult> UpdateMenuIngredient(Guid restaurant_id, short menu_id, MenuIngredientDto body)
    {
        var menu = await menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        await menuService.UpdateIngredient(restaurant_id, menu.Id, body.ingredient_id, body.amount);
        await menuService.SaveAsync();

        return NoContent();
    }

    [HttpDelete("{menu_id}")]
    public async Task<ActionResult> DeleteMenu(Guid restaurant_id, short menu_id)
    {
        var menu = await menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        await menuService.DeleteMenu(menu);
        await menuService.SaveAsync();

        return NoContent();
    }
}