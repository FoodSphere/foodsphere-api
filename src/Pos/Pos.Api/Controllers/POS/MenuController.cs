namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/menus")]
public class MenuController(
    ILogger<MenuController> logger,
    AccessControlService accessControl,
    MenuServiceBase menuService,
    MenuImageService imageService
) : PosControllerBase
{
    /// <summary>
    /// list menus
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<MenuResponse>>> ListMenus(
        Guid restaurant_id,
        [FromQuery] IReadOnlyCollection<MenuStatus> status,
        [FromQuery] bool? has_components = null,
        [FromQuery] bool? is_deleted = false)
    {
        Expression<Func<Menu, bool>> predicate = e =>
            e.RestaurantId == restaurant_id;

        if (status.Count > 0)
            predicate = predicate.And(e => status.Contains(e.Status));

        if (has_components is not null)
            predicate = predicate.And(e => e.Components.Any() == has_components.Value);

        if (is_deleted is not null)
            predicate = predicate.And(e => e.DeleteTime != null == is_deleted.Value);

        return await menuService.ListMenus(
            MenuResponse.Projection, predicate);
    }

    /// <summary>
    /// create menu
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MenuResponse>> CreateMenu(
        Guid restaurant_id, MenuRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.CREATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var tagKeys = body.tags.Select(e =>
            new TagKey(restaurant_id, e.tag_id));

        var ingredients = body.ingredients.Select(e =>
            (new IngredientKey(restaurant_id, e.ingredient_id), e.amount));

        var components = body.components.Select(e =>
            (new MenuKey(restaurant_id, e.menu_id), e.quantity));

        var createResult = await menuService.CreateMenu(
            MenuResponse.Projection, new(
                RestaurantKey: new(restaurant_id),
                Name: body.name,
                Price: body.price,
                Tags: tagKeys,
                Ingredients: ingredients,
                Components: components,
                DisplayName: body.display_name,
                Description: body.description,
                Status: body.status));

        if (createResult.IsFailed)
            return createResult.Errors.ToActionResult();

        var (key, response) = createResult.Value;

        return CreatedAtAction(
            nameof(GetMenu),
            new { restaurant_id, menu_id = key.Id },
            response);
    }

    /// <summary>
    /// get menu
    /// </summary>
    [HttpGet("{menu_id}")]
    public async Task<ActionResult<MenuResponse>> GetMenu(
        Guid restaurant_id, short menu_id)
    {
        var response = await menuService.GetMenu(
            MenuResponse.Projection,
            new(restaurant_id, menu_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// update menu
    /// </summary>
    [HttpPut("{menu_id}")]
    public async Task<ActionResult> UpdateMenu(
        Guid restaurant_id, short menu_id, MenuRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var tagKeys = body.tags.Select(e =>
            new TagKey(restaurant_id, e.tag_id));

        var ingredients = body.ingredients.Select(e =>
            (new IngredientKey(restaurant_id, e.ingredient_id), e.amount));

        var components = body.components.Select(e =>
            (new MenuKey(restaurant_id, e.menu_id), e.quantity));

        var result = await menuService.UpdateMenu(
            new(restaurant_id, menu_id), new(
                Name: body.name,
                Price: body.price,
                Tags: tagKeys,
                Ingredients: ingredients,
                Components: components,
                DisplayName: body.display_name,
                Description: body.description,
                Status: body.status));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// delete menu
    /// </summary>
    [HttpDelete("{menu_id}")]
    public async Task<ActionResult> DeleteMenu(
        Guid restaurant_id, short menu_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await menuService.DeleteMenu(
            new(restaurant_id, menu_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// upload menu image
    /// </summary>
    [HttpPost("{menu_id}/image")]
    public async Task<ActionResult<UploadImageResponse>> UploadImage(
        Guid restaurant_id, short menu_id, IFormFile file)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        ResultObject<string> uploadResult;

        using (var stream = file.OpenReadStream())
            uploadResult = await imageService.UploadImage(
                new(restaurant_id, menu_id),
                stream, file.ContentType);

        if (!uploadResult.TryGetValue(out var url))
            return uploadResult.Errors.ToActionResult();

        return new UploadImageResponse
        {
            image_url = url
        };
    }

    /// <summary>
    /// generate menu image's upload url
    /// </summary>
    [HttpPost("{menu_id}/image/upload-url")]
    public async Task<ActionResult<PresignUrlResponse>> GenerateImageUploadUrl(
        Guid restaurant_id, short menu_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await imageService.GetImageUploadUrl(
            new(restaurant_id, menu_id));

        if (!result.TryGetValue(out var url))
            return result.Errors.ToActionResult();

        return new PresignUrlResponse
        {
            url = url
        };
    }

    /// <summary>
    /// list menu's ingredients
    /// </summary>
    [HttpGet("{menu_id}/ingredients")]
    public async Task<ActionResult<ICollection<IngredientItemDto>>> ListIngredients(
        Guid restaurant_id, short menu_id)
    {
        return await menuService.ListMenuIngredients(
            IngredientItemDto.Projection, e =>
                e.RestaurantId == restaurant_id &&
                e.MenuId == menu_id);
    }

    /// <summary>
    /// update menu's ingredients
    /// </summary>
    [HttpPut("{menu_id}/ingredients")]
    public async Task<ActionResult> UpdateIngredients(
        Guid restaurant_id, short menu_id,
        IReadOnlyCollection<IngredientItemDto> body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var ingredients = body.Select(e =>
            (new IngredientKey(restaurant_id, e.ingredient_id), e.amount));

        var result = await menuService.SetIngredients(
            new(restaurant_id, menu_id),
            ingredients);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

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
            IngredientItemDto.Projection,
            new(restaurant_id, menu_id, ingredient_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// upsert specific ingredient
    /// </summary>
    [HttpPut("{menu_id}/ingredients/{ingredient_id}")]
    public async Task<ActionResult<IngredientItemDto>> UpsertIngredientItem(
        Guid restaurant_id, short menu_id, short ingredient_id,
        IngredientItemRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await menuService.SetMenuIngredient(
            IngredientItemDto.Projection,
            new(restaurant_id, menu_id),
            new(restaurant_id, ingredient_id),
            body.amount);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        if (result.Value is null)
            return NoContent();

        var (_, response) = result.Value.Value;

        return CreatedAtAction(
            nameof(GetIngredientItem),
            new { restaurant_id, menu_id, ingredient_id },
            response);
    }

    /// <summary>
    /// remove specific ingredient
    /// </summary>
    [HttpDelete("{menu_id}/ingredients/{ingredient_id}")]
    public async Task<ActionResult> DeleteIngredientItem(
        Guid restaurant_id, short menu_id, short ingredient_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await menuService.DeleteMenuIngredient(
            new(restaurant_id, menu_id, ingredient_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// list menu's tags
    /// </summary>
    [HttpGet("{menu_id}/tags")]
    public async Task<ActionResult<ICollection<TagDto>>> ListTags(
        Guid restaurant_id, short menu_id)
    {
        return await menuService.ListTags(
            TagDto.MenuTagProjection, e =>
                e.RestaurantId == restaurant_id &&
                e.MenuId == menu_id);
    }

    /// <summary>
    /// update menu's tags
    /// </summary>
    [HttpPut("{menu_id}/tags")]
    public async Task<ActionResult> UpdateTags(
        Guid restaurant_id, short menu_id, IReadOnlyCollection<short> tag_ids)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var tagKeys = tag_ids.Select(id =>
            new TagKey(restaurant_id, id));

        var result = await menuService.SetTags(
            new(restaurant_id, menu_id),
            tagKeys);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// get specific tag
    /// </summary>
    [HttpGet("{menu_id}/tags/{tag_id}")]
    public async Task<ActionResult<TagDto>> GetTagItem(
        Guid restaurant_id, short menu_id, short tag_id)
    {
        var response = await menuService.GetTagItem(
            TagDto.MenuTagProjection,
            new(restaurant_id, menu_id, tag_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// upsert specific tag
    /// </summary>
    [HttpPut("{menu_id}/tags/{tag_id}")]
    public async Task<ActionResult<TagDto>> UpsertTagItem(
        Guid restaurant_id, short menu_id, short tag_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await menuService.AssignTag(
            TagDto.MenuTagProjection,
            new(restaurant_id, menu_id),
            new(restaurant_id, tag_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        if (result.Value is null)
            return NoContent();

        var (key, response) = result.Value.Value;

        return CreatedAtAction(
            nameof(GetTagItem),
            new { restaurant_id, menu_id, tag_id },
            response);
    }

    /// <summary>
    /// remove specific tag
    /// </summary>
    [HttpDelete("{menu_id}/tags/{tag_id}")]
    public async Task<ActionResult> DeleteTagItem(
        Guid restaurant_id, short menu_id, short tag_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await menuService.DeleteTagItem(
            new(restaurant_id, menu_id, tag_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// list menu's components
    /// </summary>
    [HttpGet("{menu_id}/components")]
    public async Task<ActionResult<ICollection<MenuComponentItemResponse>>> ListComponents(
        Guid restaurant_id, short menu_id)
    {
        return await menuService.ListMenuComponents(
            MenuComponentItemResponse.Projection, e =>
                e.RestaurantId == restaurant_id &&
                e.ParentMenuId == menu_id);
    }

    /// <summary>
    /// update menu's components
    /// </summary>
    [HttpPut("{menu_id}/components")]
    public async Task<ActionResult> UpdateComponents(
        Guid restaurant_id, short menu_id,
        IReadOnlyCollection<MenuComponentItemResponse> body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var components = body.Select(e =>
            (new MenuKey(restaurant_id, e.menu_id), e.quantity));

        var result = await menuService.SetComponents(
            new(restaurant_id, menu_id),
            components);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// get specific component
    /// </summary>
    [HttpGet("{menu_id}/components/{child_id}")]
    public async Task<ActionResult<MenuComponentItemResponse>> GetComponentItem(
        Guid restaurant_id, short menu_id, short child_id)
    {
        var response = await menuService.GetMenuComponent(
            MenuComponentItemResponse.Projection,
            new(restaurant_id, menu_id, child_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// upsert specific component
    /// </summary>
    [HttpPut("{menu_id}/components/{child_id}")]
    public async Task<ActionResult<IngredientItemDto>> UpsertComponentItem(
        Guid restaurant_id, short menu_id, short child_id,
        MenuComponentItemResponse body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await menuService.SetMenuComponent(
            MenuComponentItemResponse.Projection,
            new(restaurant_id, menu_id),
            new(restaurant_id, child_id),
            body.quantity);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        if (result.Value is null)
            return NoContent();

        var (_, response) = result.Value.Value;

        return CreatedAtAction(
            nameof(GetComponentItem),
            new { restaurant_id, menu_id, child_id },
            response);
    }

    /// <summary>
    /// remove specific component
    /// </summary>
    [HttpDelete("{menu_id}/components/{child_id}")]
    public async Task<ActionResult> DeleteComponentItem(
        Guid restaurant_id, short menu_id, short child_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Menu.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await menuService.DeleteMenuComponent(
            new(restaurant_id, menu_id, child_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}