namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/menus")]
public class MenuController(
    ILogger<MenuController> logger,
    AccessControlService accessControlService,
    MenuService menuService,
    TagService tagService
) : PosControllerBase
{
    /// <summary>
    /// list menus
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MenuResponse>>> ListMenus(Guid restaurant_id)
    {
        var menus = await menuService.ListMenus(restaurant_id);

        return menus
            .Select(MenuResponse.FromModel)
            .ToList();
    }

    /// <summary>
    /// create menu
    /// </summary>
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

    /// <summary>
    /// get menu
    /// </summary>
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

    /// <summary>
    /// upload menu image
    /// </summary>
    [HttpPost("{menu_id}/image")]
    public async Task<ActionResult<UploadImageResponse>> UploadImage(Guid restaurant_id, short menu_id, IFormFile file)
    {
        // MultipartReader

        var menu = await menuService.FindMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        using var stream = file.OpenReadStream();
        var url = await menuService.UploadImage(menu, stream, file.ContentType);
        await menuService.SaveChanges();

        if (string.IsNullOrEmpty(url))
        {
            return BadRequest("failed to upload image");
        }

        return new UploadImageResponse
        {
            image_url = url
        };
    }

    /// <summary>
    /// get image upload's url
    /// </summary>
    [HttpGet("{menu_id}/image/upload-url")]
    public async Task<ActionResult<PresignUrlResponse>> GetImageUploadUrl(Guid restaurant_id, short menu_id)
    {
        var menu = await menuService.FindMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        var url = await menuService.GetImageUploadUrl(menu);

        return new PresignUrlResponse
        {
            url = url
        };
    }

    /// <summary>
    /// update menu
    /// </summary>
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

    /// <summary>
    /// update menu ingredient
    /// </summary>
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

    /// <summary>
    /// list menu's ingredients
    /// </summary>
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

    /// <summary>
    /// add tag to menu
    /// </summary>
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

    /// <summary>
    /// delete menu
    /// </summary>
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