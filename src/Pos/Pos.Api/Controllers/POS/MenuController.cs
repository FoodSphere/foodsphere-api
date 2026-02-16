namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/menus")]
public class MenuController(
    ILogger<MenuController> logger,
    AccessControlService accessControl,
    MenuService menuService,
    MenuImageService imageService
) : PosControllerBase
{
    /// <summary>
    /// list menus
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<MenuResponse>>> ListMenus(Guid restaurant_id)
    {
        var responses = await menuService.QueryMenus()
            .Where(e => e.RestaurantId == restaurant_id)
            .Select(MenuResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create menu
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MenuResponse>> CreateMenu(Guid restaurant_id, MenuRequest body)
    {
        var hasPermission = await accessControl.Validate(
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

        foreach (var item in body.ingredients)
        {
            await menuService.AssignIngredient(menu, item.ingredient_id, item.amount);
        }

        foreach (var tag in body.tags)
        {
            await menuService.CreateMenuTag(menu, tag.tag_id);
        }

        await menuService.SaveChanges();

        var response = await menuService.GetMenu(
            restaurant_id, menu.Id,
            MenuResponse.Projection);

        return CreatedAtAction(
            nameof(GetMenu),
            new { restaurant_id, menu_id = menu.Id },
            response);
    }

    /// <summary>
    /// get menu
    /// </summary>
    [HttpGet("{menu_id}")]
    public async Task<ActionResult<MenuResponse>> GetMenu(Guid restaurant_id, short menu_id)
    {
        var response = await menuService.GetMenu(
            restaurant_id, menu_id,
            MenuResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// update menu
    /// </summary>
    [HttpPut("{menu_id}")]
    public async Task<ActionResult> UpdateMenu(Guid restaurant_id, short menu_id, MenuRequest body)
    {
        var hasPermission = await accessControl.Validate(
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
        menu.DisplayName = body.display_name;
        menu.Description = body.description;

        await menuService.QueryMenuIngredients(restaurant_id, menu_id)
            .ExecuteDeleteAsync();

        await menuService.QueryMenuTags(restaurant_id, menu_id)
            .ExecuteDeleteAsync();

        foreach (var item in body.ingredients)
        {
            await menuService.AssignIngredient(menu, item.ingredient_id, item.amount);
        }

        foreach (var tag in body.tags)
        {
            await menuService.CreateMenuTag(menu, tag.tag_id);
        }

        await menuService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// delete menu
    /// </summary>
    [HttpDelete("{menu_id}")]
    public async Task<ActionResult> DeleteMenu(Guid restaurant_id, short menu_id)
    {
        var affected = await menuService
            .QuerySingleMenu(restaurant_id, menu_id)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// upload menu image
    /// </summary>
    [HttpPost("{menu_id}/image")]
    public async Task<ActionResult<UploadImageResponse>> UploadImage(Guid restaurant_id, short menu_id, IFormFile file)
    {
        var menu = await menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        string? url;

        using var stream = file.OpenReadStream();
        {
            url = await imageService.UploadImage(menu, stream, file.ContentType);
        }

        if (url is null)
        {
            return BadRequest("failed to upload image");
        }

        await menuService.SaveChanges();

        return new UploadImageResponse
        {
            image_url = url
        };
    }

    /// <summary>
    /// generate menu image's upload url
    /// </summary>
    [HttpPost("{menu_id}/image/upload-url")]
    public async Task<ActionResult<PresignUrlResponse>> GenerateImageUploadUrl(Guid restaurant_id, short menu_id)
    {
        var menu = await menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        var url = await imageService.GetImageUploadUrl(menu);

        return new PresignUrlResponse
        {
            url = url
        };
    }

    /// <summary>
    /// list menu's ingredients
    /// </summary>
    [HttpGet("{menu_id}/ingredients")]
    public async Task<ActionResult<ICollection<IngredientItemDto>>> ListIngredients(Guid restaurant_id, short menu_id)
    {
        var responses = await menuService
            .QueryMenuIngredients(restaurant_id, menu_id)
            .Select(IngredientItemDto.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// update menu's ingredients
    /// </summary>
    [HttpPut("{menu_id}/ingredients")]
    public async Task<ActionResult> UpdateIngredients(
        Guid restaurant_id, short menu_id,
        IReadOnlyCollection<IngredientItemDto> body)
    {
        var menu = await menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        await menuService
            .QueryMenuIngredients(restaurant_id, menu_id)
            .ExecuteDeleteAsync();

        foreach (var item in body)
        {
            await menuService.AssignIngredient(menu, item.ingredient_id, item.amount);
        }

        await menuService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// get specific ingredient
    /// </summary>
    [HttpGet("{menu_id}/ingredients/{ingredient_id}")]
    public async Task<ActionResult<IngredientItemDto>> GetIngredientItem(
        Guid restaurant_id, short menu_id, short ingredient_id)
    {
        var response = await menuService.GetMenuIngredient(
            restaurant_id, menu_id, ingredient_id,
            IngredientItemDto.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// upsert specific ingredient
    /// </summary>
    [HttpPut("{menu_id}/ingredients/{ingredient_id}")]
    public async Task<ActionResult<IngredientItemDto>> UpsertIngredientItem(
        Guid restaurant_id,
        short menu_id,
        short ingredient_id,
        IngredientItemRequest body)
    {
        var menuIngredient = await menuService.GetMenuIngredient(restaurant_id, menu_id, ingredient_id);

        if (menuIngredient is null)
        {
            var menu = await menuService.GetMenu(restaurant_id, menu_id);

            if (menu is null)
            {
                return NotFound();
            }

            menuIngredient = await menuService.AssignIngredient(menu, ingredient_id, body.amount);

            await menuService.SaveChanges();

            var response = await menuService.GetMenuIngredient(
                restaurant_id, menu_id, ingredient_id,
                IngredientItemDto.Projection);

            return CreatedAtAction(
                nameof(GetIngredientItem),
                new { restaurant_id, menu_id, ingredient_id },
                response);
        }

        menuIngredient.Amount = body.amount;

        await menuService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// remove specific ingredient
    /// </summary>
    [HttpDelete("{menu_id}/ingredients/{ingredient_id}")]
    public async Task<ActionResult> DeleteIngredientItem(Guid restaurant_id, short menu_id, short ingredient_id)
    {
        var affected = await menuService
            .QuerySingleMenuIngredient(restaurant_id, menu_id, ingredient_id)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// list menu's tags
    /// </summary>
    [HttpGet("{menu_id}/tags")]
    public async Task<ActionResult<ICollection<TagDto>>> ListTags(Guid restaurant_id, short menu_id)
    {
        var responses = await menuService
            .QueryMenuTags(restaurant_id, menu_id)
            .Select(TagDto.MenuTagProjection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// update menu's tags
    /// </summary>
    [HttpPut("{menu_id}/tags")]
    public async Task<ActionResult> UpdateTags(Guid restaurant_id, short menu_id, IReadOnlyCollection<short> tag_ids)
    {
        var menu = await menuService.GetMenu(restaurant_id, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        await menuService
            .QueryMenuTags(restaurant_id, menu_id)
            .ExecuteDeleteAsync();

        foreach (var id in tag_ids)
        {
            await menuService.CreateMenuTag(menu, id);
        }

        await menuService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// get specific tag
    /// </summary>
    [HttpGet("{menu_id}/tags/{tag_id}")]
    public async Task<ActionResult<TagDto>> GetTagItem(Guid restaurant_id, short menu_id, short tag_id)
    {
        var response = await menuService.GetMenuTag(
            restaurant_id, menu_id, tag_id,
            TagDto.MenuTagProjection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// upsert specific tag
    /// </summary>
    [HttpPut("{menu_id}/tags/{tag_id}")]
    public async Task<ActionResult<TagDto>> UpsertTagItem(Guid restaurant_id, short menu_id, short tag_id)
    {
        var menuTag = await menuService.GetMenuTag(restaurant_id, menu_id, tag_id);

        if (menuTag is null)
        {
            var menu = await menuService.GetMenu(restaurant_id, menu_id);

            if (menu is null)
            {
                return NotFound();
            }

            menuTag = await menuService.CreateMenuTag(menu, tag_id);

            await menuService.SaveChanges();

            var response = await menuService.GetMenuTag(
                restaurant_id, menu_id, tag_id,
                TagDto.MenuTagProjection);

            return CreatedAtAction(
                nameof(GetTagItem),
                new { restaurant_id, menu_id, tag_id },
                response);
        }

        return NoContent();
    }

    /// <summary>
    /// remove specific tag
    /// </summary>
    [HttpDelete("{menu_id}/tags/{tag_id}")]
    public async Task<ActionResult> DeleteTagItem(Guid restaurant_id, short menu_id, short tag_id)
    {
        var affected = await menuService
            .QuerySingleMenuTag(restaurant_id, menu_id, tag_id)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }
}