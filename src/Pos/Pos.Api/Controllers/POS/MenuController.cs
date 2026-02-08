namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/menus")]
public class MenuController(
    ILogger<MenuController> logger,
    AccessControlService accessControlService,
    MenuService menuService,
    TagService tagService
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
        var hasPermission = await accessControlService.Validate(
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
            description: body.description
        );

        foreach (var ingredient in body.ingredients)
        {
            await menuService.UpdateIngredient(restaurant_id, menu.Id, ingredient.ingredient_id, ingredient.amount);
        }

        await menuService.SaveChanges();

        return CreatedAtAction(
            nameof(GetMenu),
            new { restaurant_id, menu_id = menu.Id },
            MenuResponse.FromModel(menu)
        );
    }

    [HttpGet("{menu_id}")]
    public async Task<ActionResult<MenuResponse>> GetMenu(Guid restaurant_id, short menu_id)
    {
        var menu = await menuService.FindMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        return MenuResponse.FromModel(menu);
    }

    [HttpPut("{menu_id}")]
    public async Task<ActionResult> UpdateMenu(Guid restaurant_id, short menu_id, MenuRequest body)
    {
        var hasPermission = await accessControlService.Validate(
            User, restaurant_id,
            PERMISSION.Menu.UPDATE
        );

        if (!hasPermission)
        {
            return Forbid();
        }

        var menu = await menuService.FindMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        menu.Name = body.name;
        menu.Price = body.price;
        menu.DisplayName = body?.display_name;
        menu.Description = body?.description;

        await menuService.SaveChanges();

        return NoContent();
    }

    [HttpPost("{menu_id}/ingredients")]
    public async Task<ActionResult> UpdateMenuIngredient(Guid restaurant_id, short menu_id, IngredientItemDto body)
    {
        var menu = await menuService.FindMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        await menuService.UpdateIngredient(restaurant_id, menu.Id, body.ingredient_id, body.amount);
        await menuService.SaveChanges();

        return NoContent();
    }

    [HttpGet("{menu_id}/ingredients")]
    public async Task<ActionResult<IngredientItemDto[]>> ListIngredients(Guid restaurant_id, short menu_id)
    {
        var menu = await menuService.FindMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        return menu.MenuIngredients
            .Select(IngredientItemDto.FromModel)
            .ToArray();
    }

    [HttpPost("{menu_id}/tags")]
    public async Task<ActionResult> AddTag(Guid restaurant_id, short menu_id, AssignTagRequest body)
    {
        var menu = await menuService.FindMenu(restaurant_id, menu_id);
        var tag = await tagService.GetTag(restaurant_id, body.tag_id);

        if (tag is null || menu is null)
        {
            return NotFound();
        }

        await menuService.AddTag(menu, tag);
        await menuService.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{menu_id}")]
    public async Task<ActionResult> DeleteMenu(Guid restaurant_id, short menu_id)
    {
        var menu = await menuService.FindMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        await menuService.DeleteMenu(menu);
        await menuService.SaveChanges();

        return NoContent();
    }
}