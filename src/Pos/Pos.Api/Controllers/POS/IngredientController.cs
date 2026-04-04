namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/ingredients")]
public class IngredientController(
    ILogger<IngredientController> logger,
    AccessControlService accessControl,
    IngredientServiceBase ingredientService,
    IngredientImageService imageService
) : PosControllerBase
{
    /// <summary>
    /// list ingredients
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<IngredientResponse>>> ListIngredients(
        Guid restaurant_id,
        [FromQuery] bool? is_deleted = false)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        Expression<Func<Ingredient, bool>> predicate = e =>
            e.RestaurantId == restaurant_id;

        if (is_deleted is not null)
            predicate = predicate.And(e =>
                e.DeleteTime != null == is_deleted.Value);

        return await ingredientService.ListIngredients(
            IngredientResponse.Projection, predicate);
    }

    /// <summary>
    /// create ingredient
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<IngredientResponse>> CreateIngredient(
        Guid restaurant_id, IngredientRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.CREATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var tagKeys = body.tags.Select(e =>
            new TagKey(restaurant_id, e.tag_id));

        var createResult = await ingredientService.CreateIngredient(
            IngredientResponse.Projection, new(
                RestaurantKey: new(restaurant_id),
                Name: body.name,
                Tags: tagKeys,
                Unit: body.unit,
                Description: body.description,
                Status: body.status));

        if (createResult.IsFailed)
            return createResult.Errors.ToActionResult();

        var (key, response) = createResult.Value;

        return CreatedAtAction(
            nameof(GetIngredient),
            new { restaurant_id, ingredient_id = key.Id },
            response);
    }

    /// <summary>
    /// get ingredient
    /// </summary>
    [HttpGet("{ingredient_id}")]
    public async Task<ActionResult<IngredientResponse>> GetIngredient(
        Guid restaurant_id, short ingredient_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var response = await ingredientService.GetIngredient(
            IngredientResponse.Projection,
            new(restaurant_id, ingredient_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// update ingredient
    /// </summary>
    [HttpPut("{ingredient_id}")]
    public async Task<ActionResult> UpdateIngredient(
        Guid restaurant_id, short ingredient_id, IngredientRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var tagKeys = body.tags.Select(e =>
            new TagKey(restaurant_id, e.tag_id));

        var result = await ingredientService.UpdateIngredient(
            new(restaurant_id, ingredient_id), new(
                Name: body.name,
                Tags: tagKeys,
                Unit: body.unit,
                Description: body.description,
                Status: body.status));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// delete ingredient
    /// </summary>
    [HttpDelete("{ingredient_id}")]
    public async Task<ActionResult> DeleteIngredient(
        Guid restaurant_id, short ingredient_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await ingredientService.DeleteIngredient(
            new(restaurant_id, ingredient_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// upload ingredient image
    /// </summary>
    [HttpPost("{ingredient_id}/image")]
    public async Task<ActionResult<UploadImageResponse>> UploadImage(
        Guid restaurant_id, short ingredient_id,
        IFormFile file)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        ResultObject<string> uploadResult;

        using (var stream = file.OpenReadStream())
            uploadResult = await imageService.UploadImage(
                new(restaurant_id, ingredient_id),
                stream, file.ContentType);

        if (!uploadResult.TryGetValue(out var url))
            return uploadResult.Errors.ToActionResult();

        return new UploadImageResponse
        {
            image_url = url
        };
    }

    /// <summary>
    /// generate ingredient image's upload url
    /// </summary>
    [HttpPost("{ingredient_id}/image/upload-url")]
    public async Task<ActionResult<PresignUrlResponse>> GenerateImageUploadUrl(
        Guid restaurant_id, short ingredient_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await imageService.GetImageUploadUrl(
            new(restaurant_id, ingredient_id));

        if (!result.TryGetValue(out var url))
            return result.Errors.ToActionResult();

        return new PresignUrlResponse
        {
            url = url
        };
    }

    /// <summary>
    /// list ingredient's tags
    /// </summary>
    [HttpGet("{ingredient_id}/tags")]
    public async Task<ActionResult<ICollection<TagDto>>> ListTags(
        Guid restaurant_id, short ingredient_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        return await ingredientService.ListTags(
            TagDto.IngredientTagProjection, e =>
                e.RestaurantId == restaurant_id &&
                e.IngredientId == ingredient_id);
    }

    /// <summary>
    /// update ingredient's tags
    /// </summary>
    [HttpPut("{ingredient_id}/tags")]
    public async Task<ActionResult> UpdateTags(
        Guid restaurant_id, short ingredient_id,
        IReadOnlyCollection<short> tag_ids)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var tagKeys = tag_ids.Select(id =>
            new TagKey(restaurant_id, id));

        var result = await ingredientService.SetTags(
            new(restaurant_id, ingredient_id),
            tagKeys);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// get specific tag
    /// </summary>
    [HttpGet("{ingredient_id}/tags/{tag_id}")]
    public async Task<ActionResult<TagDto>> GetTagItem(
        Guid restaurant_id, short ingredient_id, short tag_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var response = await ingredientService.GetTagItem(
            TagDto.IngredientTagProjection,
            new(restaurant_id, ingredient_id, tag_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// upsert specific tag
    /// </summary>
    [HttpPut("{ingredient_id}/tags/{tag_id}")]
    public async Task<ActionResult<TagDto>> UpsertTagItem(
        Guid restaurant_id, short ingredient_id, short tag_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await ingredientService.AssignTag(
            TagDto.IngredientTagProjection,
            new(restaurant_id, ingredient_id),
            new(restaurant_id, tag_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        if (result.Value is null)
            return NoContent();

        var (key, response) = result.Value.Value;

        return CreatedAtAction(
            nameof(GetTagItem),
            new { restaurant_id, ingredient_id, tag_id },
            response);
    }

    /// <summary>
    /// remove specific tag
    /// </summary>
    [HttpDelete("{ingredient_id}/tags/{tag_id}")]
    public async Task<ActionResult> DeleteTagItem(
        Guid restaurant_id, short ingredient_id, short tag_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Ingredient.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await ingredientService.DeleteTagItem(
            new(restaurant_id, ingredient_id, tag_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}