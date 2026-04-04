namespace FoodSphere.Infrastructure.Repository;

public class IngredientRepository(
    FoodSphereDbContext context
) : RepositoryBase(context)
{
    public async Task<ResultObject<Ingredient>> CreateIngredient(
        RestaurantKey restaurantKey,
        string name,
        string? unit,
        string? description,
        string? imageUrl,
        CancellationToken ct = default)
    {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Ingredient>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.RestaurantId == restaurantKey.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Ingredient>().Local
                .Where(e => e.RestaurantId == restaurantKey.Id)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Ingredient>()
                .Where(e => e.RestaurantId == restaurantKey.Id)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var ingredient = new Ingredient
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantKey.Id,
            Name = name,
            Unit = unit,
            Description = description,
            ImageUrl = imageUrl,
        };

        _ctx.Add(ingredient);

        return ingredient;
    }

    Ingredient CreateIngredientStub(IngredientKey key)
    {
        var ingredient = new Ingredient
        {
            RestaurantId = key.RestaurantId,
            Id = key.Id,
            Name = default!,
        };

        _ctx.Attach(ingredient);

        return ingredient;
    }

    public IQueryable<Ingredient> QueryIngredients()
    {
        return _ctx.Set<Ingredient>()
            .AsExpandable();
    }

    public IQueryable<Ingredient> QuerySingleIngredient(IngredientKey key)
    {
        return QueryIngredients()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.Id == key.Id);
    }

    public async Task<Ingredient?> GetIngredient(
        IngredientKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<Ingredient>(key, ct);
    }

    public async Task<ResultObject> DeleteIngredient(
        IngredientKey key, CancellationToken ct = default)
    {
        var ingredient = await GetIngredient(key, ct);

        if (ingredient is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(ingredient);

        return ResultObject.Success();
    }

    public async Task<ResultObject<TagIngredient>> CreateIngredientTag(
        IngredientKey ingredientKey, TagKey tagKey,
        CancellationToken ct = default)
    {
        var ingredient = await GetIngredient(ingredientKey, ct);

        if (ingredient is null)
            return ResultObject.NotFound(ingredientKey);

        var tag = await _ctx.FindAsync<Tag>(tagKey, ct);

        if (tag is null)
            return ResultObject.NotFound(tagKey);

        var ingredientTag = new TagIngredient
        {
            RestaurantId = ingredientKey.RestaurantId,
            IngredientId = ingredientKey.Id,
            TagId = tagKey.Id,
        };

        _ctx.Add(ingredientTag);

        return ingredientTag;
    }

    public IQueryable<TagIngredient> QueryIngredientTags()
    {
        return _ctx.Set<TagIngredient>()
            .AsExpandable();
    }

    public IQueryable<TagIngredient> QueryIngredientTags(IngredientKey key)
    {
        return QueryIngredientTags()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.IngredientId == key.Id);
    }

    public IQueryable<TagIngredient> QuerySingleIngredientTag(IngredientTagKey key)
    {
        return QueryIngredientTags()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.IngredientId == key.IngredientId &&
                e.TagId == key.TagId);
    }

    public async Task<TagIngredient?> GetIngredientTag(
        IngredientTagKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<TagIngredient>(key, ct);
    }

    public async Task<ResultObject> DeleteIngredientTag(
        IngredientTagKey key, CancellationToken ct = default)
    {
        var ingredientTag = await GetIngredientTag(key, ct);

        if (ingredientTag is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(ingredientTag);

        return ResultObject.Success();
    }

    public async Task<ResultObject> SetTags(
        IngredientKey ingredientKey,
        IEnumerable<TagKey> tagKeys,
        CancellationToken ct = default)
    {
        var desired = tagKeys
            .Distinct()
            .ToArray();

        var existed = await QueryIngredientTags(ingredientKey)
            .ToArrayAsync(ct);

        var toRemove = existed
            .ExceptBy(desired.Select(e => e.Id), e => e.TagId);

        var toAdd = desired
            .ExceptBy(existed.Select(e => e.TagId), e => e.Id);

        foreach (var tagKey in toAdd)
            await CreateIngredientTag(ingredientKey, tagKey, ct);

        foreach (var tag in toRemove)
            await DeleteIngredientTag(tag, ct);

        return ResultObject.Success();
    }
}