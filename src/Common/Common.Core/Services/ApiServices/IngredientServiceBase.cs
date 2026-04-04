namespace FoodSphere.Common.Service;

public class IngredientServiceBase(
    PersistenceService persistenceService,
    IngredientRepository ingredientRepository,
    MenuRepository menuRepository,
    IngredientImageService imageService
) : ServiceBase
{
    public async Task<TDto[]> ListIngredients<TDto>(
        Expression<Func<Ingredient, TDto>> projection,
        Expression<Func<Ingredient, bool>> predicate,
        CancellationToken ct = default)
    {
        return await ingredientRepository.QueryIngredients()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(IngredientKey, TDto)>> CreateIngredient<TDto>(
        Expression<Func<Ingredient, TDto>> projection,
        IngredientCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await ingredientRepository.CreateIngredient(
            restaurantKey: command.RestaurantKey,
            name: command.Name,
            unit: command.Unit,
            description: command.Description,
            imageUrl: null,
            ct);

        if (!createResult.TryGetValue(out var ingredient))
            return createResult.Errors;

        ingredient.Status = command.Status;

        var tagResult = await ingredientRepository.SetTags(
            ingredient, command.Tags, ct);

        if (tagResult.IsFailed)
            return tagResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetIngredient(projection, ingredient, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created ingredient.");

        return (ingredient, response);
    }

    public async Task<TDto?> GetIngredient<TDto>(
        Expression<Func<Ingredient, TDto>> projection, IngredientKey key,
        CancellationToken ct = default)
    {
        return await ingredientRepository.QuerySingleIngredient(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateIngredient(
        IngredientKey key, IngredientUpdateCommand command,
        CancellationToken ct = default)
    {
        var ingredient = await ingredientRepository.GetIngredient(key, ct);

        if (ingredient is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Ingredient not found.");

        ingredient.Name = command.Name;
        ingredient.Unit = command.Unit;
        ingredient.Description = command.Description;
        ingredient.Status = command.Status;

        var tagResult = await ingredientRepository.SetTags(
            ingredient, command.Tags, ct);

        if (tagResult.IsFailed)
            return tagResult.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteIngredient(
        IngredientKey key, CancellationToken ct = default)
    {
        var ingredient = await ingredientRepository.GetIngredient(key, ct);

        if (ingredient is null)
            return ResultObject.NotFound(key);

        var deleteResult = await ingredientRepository.DeleteIngredient(key, ct);

        if (deleteResult.IsFailed)
            return deleteResult.Errors;

        var imageResult = await imageService.DeleteImage(key, ct);

        if (imageResult.IsFailed)
            return imageResult.Errors;

        await menuRepository.QueryMenuIngredients()
            .Where(e => e.RestaurantId == key.RestaurantId)
            .Where(e => e.IngredientId == key.Id)
            .ExecuteDeleteAsync(ct);

        var result = await ingredientRepository.DeleteIngredient(key, ct);

        if (imageResult.IsFailed)
            return imageResult.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> SetTags(
        IngredientKey key, IEnumerable<TagKey> tagKeys,
        CancellationToken ct = default)
    {
        var result = await ingredientRepository.SetTags(
            key, tagKeys, ct);

        if (result.IsFailed)
            return result.Errors;

        return ResultObject.Success();
    }

    public async Task<TDto[]> ListTags<TDto>(
        Expression<Func<TagIngredient, TDto>> projection,
        Expression<Func<TagIngredient, bool>> predicate,
        CancellationToken ct = default)
    {
        return await ingredientRepository.QueryIngredientTags()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(IngredientTagKey, TDto)?>> AssignTag<TDto>(
        Expression<Func<TagIngredient, TDto>> projection,
        IngredientKey ingredientKey, TagKey tagKey,
        CancellationToken ct = default)
    {
        var ingredientTag = await ingredientRepository.GetIngredientTag(
            new(ingredientKey.RestaurantId, ingredientKey.Id, tagKey.Id),
            ct);

        if (ingredientTag is not null)
            return ResultObject.None();

        var createResult = await ingredientRepository.CreateIngredientTag(
            ingredientKey, tagKey, ct);

        if (!createResult.TryGetValue(out ingredientTag))
            return createResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetTagItem(
            projection, ingredientTag, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created tag.");

        return (ingredientTag, response);
    }

    public async Task<TDto?> GetTagItem<TDto>(
        Expression<Func<TagIngredient, TDto>> projection,
        IngredientTagKey key,
        CancellationToken ct = default)
    {
        return await ingredientRepository.QuerySingleIngredientTag(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> DeleteTagItem(
        IngredientTagKey key, CancellationToken ct = default)
    {
        var result = await ingredientRepository.DeleteIngredientTag(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}