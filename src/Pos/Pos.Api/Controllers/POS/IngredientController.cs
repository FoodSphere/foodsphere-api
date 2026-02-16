namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/ingredients")]
public class IngredientController(
    ILogger<IngredientController> logger,
    AccessControlService accessControl,
    IngredientService ingredientService,
    IngredientImageService imageService
) : PosControllerBase
{
    /// <summary>
    /// list ingredients
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<IngredientResponse>>> ListIngredients(Guid restaurant_id)
    {
        var hasPermission = await accessControl.Validate(
            User, restaurant_id,
            PERMISSION.Ingredient.READ
        );

        if (!hasPermission)
        {
            return Forbid();
        }

        var responses = await ingredientService
            .IngredientQuery(restaurant_id)
            .Select(IngredientResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create ingredient
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<IngredientResponse>> CreateIngredient(Guid restaurant_id, IngredientRequest body)
    {
        var hasPermission = await accessControl.Validate(
            User, restaurant_id,
            PERMISSION.Ingredient.CREATE
        );

        if (!hasPermission)
        {
            return Forbid();
        }

        var ingredient = await ingredientService.CreateIngredient(
            restaurantId: restaurant_id,
            name: body.name,
            unit: body.unit,
            description: body.description
        );

        foreach (var tag in body.tags)
        {
            await ingredientService.AssignTag(ingredient, tag.tag_id);
        }

        await ingredientService.SaveChanges();

        var response = await ingredientService.GetIngredient(
            restaurant_id, ingredient.Id,
            IngredientResponse.Projection);

        return CreatedAtAction(
            nameof(GetIngredient),
            new { restaurant_id, ingredient_id = ingredient.Id },
            response);
    }

    /// <summary>
    /// get ingredient
    /// </summary>
    [HttpGet("{ingredient_id}")]
    public async Task<ActionResult<IngredientResponse>> GetIngredient(Guid restaurant_id, short ingredient_id)
    {
        var hasPermission = await accessControl.Validate(
            User, restaurant_id,
            PERMISSION.Ingredient.READ
        );

        if (!hasPermission)
        {
            return Forbid();
        }

        var response = await ingredientService.GetIngredient(
            restaurant_id, ingredient_id,
            IngredientResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// update ingredient
    /// </summary>
    [HttpPut("{ingredient_id}")]
    public async Task<ActionResult> UpdateIngredient(Guid restaurant_id, short ingredient_id, IngredientRequest body)
    {
        var hasPermission = await accessControl.Validate(
            User, restaurant_id,
            PERMISSION.Ingredient.UPDATE
        );

        if (!hasPermission)
        {
            return Forbid();
        }

        var ingredient = await ingredientService.GetIngredient(restaurant_id, ingredient_id);

        if (ingredient is null)
        {
            return NotFound();
        }

        ingredient.Name = body.name;
        ingredient.Unit = body.unit;
        ingredient.Description = body.description;

        await ingredientService
            .IngredientTagQuery(restaurant_id, ingredient_id)
            .ExecuteDeleteAsync();

        foreach (var tag in body.tags)
        {
            await ingredientService.AssignTag(ingredient, tag.tag_id);
        }

        await ingredientService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// delete ingredient
    /// </summary>
    [HttpDelete("{ingredient_id}")]
    public async Task<ActionResult> DeleteIngredient(Guid restaurant_id, short ingredient_id)
    {
        var affected = await ingredientService
            .IngredientQuery(restaurant_id, ingredient_id)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

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
        var ingredient = await ingredientService.GetIngredient(restaurant_id, ingredient_id);

        if (ingredient is null)
        {
            return NotFound();
        }

        string? url;

        using var stream = file.OpenReadStream();
        {
            url = await imageService.UploadImage(ingredient, stream, file.ContentType);
        }

        if (url is null)
        {
            return BadRequest("failed to upload image");
        }

        await ingredientService.SaveChanges();

        return new UploadImageResponse
        {
            image_url = url
        };
    }

    /// <summary>
    /// generate ingredient image's upload url
    /// </summary>
    [HttpPost("{ingredient_id}/image/upload-url")]
    public async Task<ActionResult<PresignUrlResponse>> GenerateImageUploadUrl(Guid restaurant_id, short ingredient_id)
    {
        var ingredient = await ingredientService.GetIngredient(restaurant_id, ingredient_id);

        if (ingredient is null)
        {
            return NotFound();
        }

        var url = await imageService.GetImageUploadUrl(ingredient);

        return new PresignUrlResponse
        {
            url = url
        };
    }

    /// <summary>
    /// list ingredient's tags
    /// </summary>
    [HttpGet("{ingredient_id}/tags")]
    public async Task<ActionResult<ICollection<TagDto>>> ListTags(Guid restaurant_id, short ingredient_id)
    {
        var responses = await ingredientService
            .IngredientTagQuery(restaurant_id, ingredient_id)
            .Select(TagDto.IngredientTagProjection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// update ingredient's tags
    /// </summary>
    [HttpPut("{ingredient_id}/tags")]
    public async Task<ActionResult> UpdateTags(
        Guid restaurant_id, short ingredient_id,
        IReadOnlyCollection<short> tag_ids)
    {
        var ingredient = await ingredientService.GetIngredient(restaurant_id, ingredient_id);

        if (ingredient is null)
        {
            return NotFound();
        }

        await ingredientService
            .IngredientTagQuery(restaurant_id, ingredient_id)
            .ExecuteDeleteAsync();

        foreach (var id in tag_ids)
        {
            await ingredientService.AssignTag(ingredient, id);
        }

        await ingredientService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// get specific tag
    /// </summary>
    [HttpGet("{ingredient_id}/tags/{tag_id}")]
    public async Task<ActionResult<TagDto>> GetTagItem(Guid restaurant_id, short ingredient_id, short tag_id)
    {
        var response = await ingredientService.GetIngredientTag(
            restaurant_id, ingredient_id, tag_id,
            TagDto.IngredientTagProjection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// upsert specific tag
    /// </summary>
    [HttpPut("{ingredient_id}/tags/{tag_id}")]
    public async Task<ActionResult<TagDto>> UpsertTagItem(Guid restaurant_id, short ingredient_id, short tag_id)
    {
        var ingredientTag = await ingredientService.GetIngredientTag(restaurant_id, ingredient_id, tag_id);

        if (ingredientTag is null)
        {
            var ingredient = await ingredientService.GetIngredient(restaurant_id, ingredient_id);

            if (ingredient is null)
            {
                return NotFound();
            }

            ingredientTag = await ingredientService.AssignTag(ingredient, tag_id);

            await ingredientService.SaveChanges();

            var response = await ingredientService.GetIngredientTag(
                restaurant_id, ingredient_id, tag_id,
                TagDto.IngredientTagProjection);

            return CreatedAtAction(
                nameof(GetTagItem),
                new { restaurant_id, ingredient_id, tag_id },
                response);
        }

        return NoContent();
    }

    /// <summary>
    /// remove specific tag
    /// </summary>
    [HttpDelete("{ingredient_id}/tags/{tag_id}")]
    public async Task<ActionResult> DeleteTagItem(Guid restaurant_id, short ingredient_id, short tag_id)
    {
        var affected = await ingredientService
            .IngredientTagQuery(restaurant_id, ingredient_id, tag_id)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }
}