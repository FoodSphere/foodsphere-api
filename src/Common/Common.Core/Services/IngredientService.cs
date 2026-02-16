namespace FoodSphere.Common.Service;

public class IngredientService(FoodSphereDbContext context) : ServiceBase(context)
{
    public async Task<Ingredient> CreateIngredient(
        Guid restaurantId,
        string name,
        string? unit = null,
        string? description = null,
        string? imageUrl = null,
        CancellationToken ct = default
    ) {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Ingredient>()
            .Any(e =>
            e.State == EntityState.Added &&
            e.Entity.RestaurantId == restaurantId);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Ingredient>().Local
                .Where(e => e.RestaurantId == restaurantId)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Ingredient>()
                .Where(e => e.RestaurantId == restaurantId)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var ingredient = new Ingredient
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantId,
            Name = name,
            Unit = unit,
            Description = description,
            ImageUrl = imageUrl,
        };

        _ctx.Add(ingredient);

        return ingredient;
    }

    public IQueryable<Ingredient> IngredientQuery(Guid restaurantId)
    {
        return _ctx.Set<Ingredient>()
            .AsExpandable()
            .Where(e => e.RestaurantId == restaurantId);
    }

    public IQueryable<Ingredient> IngredientQuery(Guid restaurantId, short ingredientId)
    {
        return IngredientQuery(restaurantId)
            .Where(e => e.Id == ingredientId);
    }

    public async Task<TDto?> GetIngredient<TDto>(Guid restaurantId, short ingredientId, Expression<Func<Ingredient, TDto>> projection)
    {
        return await IngredientQuery(restaurantId, ingredientId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<Ingredient?> GetIngredient(Guid restaurantId, short ingredientId)
    {
        var existed = await _ctx.Set<Ingredient>()
            .AnyAsync(e =>
                e.RestaurantId == restaurantId &&
                e.Id == ingredientId);

        if (!existed)
        {
            return null;
        }

        var ingredient = new Ingredient
        {
            Id = ingredientId,
            RestaurantId = restaurantId,
            Name = default!,
        };

        _ctx.Attach(ingredient);

        return ingredient;
    }

    public async Task DeleteIngredient(Ingredient ingredient)
    {
        _ctx.Remove(ingredient);
    }

    public IQueryable<IngredientTag> IngredientTagQuery(Guid restaurantId, short ingredientId)
    {
        return _ctx.Set<IngredientTag>()
            .AsExpandable()
            .Where(e =>
                e.RestaurantId == restaurantId &&
                e.IngredientId == ingredientId);
    }

    public IQueryable<IngredientTag> IngredientTagQuery(Ingredient ingredient)
    {
        return IngredientTagQuery(ingredient.RestaurantId, ingredient.Id);
    }

    public IQueryable<IngredientTag> IngredientTagQuery(Guid restaurantId, short ingredientId, short tagId)
    {
        return IngredientTagQuery(restaurantId, ingredientId)
            .Where(e => e.TagId == tagId);
    }

    public async Task<TDto?> GetIngredientTag<TDto>(Guid restaurantId, short ingredientId, short tagId, Expression<Func<IngredientTag, TDto>> projection)
    {
        return await IngredientTagQuery(restaurantId, ingredientId, tagId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<IngredientTag?> GetIngredientTag(Guid restaurantId, short ingredientId, short tagId)
    {
        return await _ctx.FindAsync<IngredientTag>(restaurantId, ingredientId, tagId);
    }

    public async Task<IngredientTag> AssignTag(Ingredient ingredient, short tagId)
    {
        var ingredientTag = new IngredientTag
        {
            Ingredient = ingredient,
            TagId = tagId
        };

        _ctx.Add(ingredientTag);

        return ingredientTag;
    }

    public async Task DeleteTag(IngredientTag ingredientTag)
    {
        _ctx.Remove(ingredientTag);
    }
}