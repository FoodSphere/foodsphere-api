using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Service;

public class IngredientService(FoodSphereDbContext context) : ServiceBase(context)
{
    public async Task<Ingredient[]> ListIngredients(Guid restaurantId)
    {
        return await _ctx.Set<Ingredient>()
            .Where(ingredient => ingredient.RestaurantId == restaurantId)
            .Include(e => e.IngredientTags)
                .ThenInclude(e => e.Tag)
            .ToArrayAsync();
    }

    public async Task<Ingredient> CreateIngredient(
        Guid restaurantId,
        string name,
        string? description = null,
        string? imageUrl = null,
        string? unit = null,
        CancellationToken ct = default
    ) {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Ingredient>()
            .Any(e => e.State == EntityState.Added && e.Entity.RestaurantId == restaurantId);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Ingredient>().Local
                .Where(ingredient => ingredient.RestaurantId == restaurantId)
                .Max(ingredient => (int?)ingredient.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Ingredient>()
                .Where(ingredient => ingredient.RestaurantId == restaurantId)
                .Select(ingredient => (int?)ingredient.Id)
                .MaxAsync(ct) ?? 0;
        }

        var ingredient = new Ingredient
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantId,
            Name = name,
            Description = description,
            ImageUrl = imageUrl,
            Unit = unit
        };

        _ctx.Add(ingredient);

        return ingredient;
    }

    public async Task<Ingredient?> FindIngredient(Guid restaurantId, short ingredientId)
    {
        return await _ctx.FindAsync<Ingredient>(restaurantId, ingredientId);
    }

    public async Task<Ingredient?> GetIngredient(Guid restaurantId, short ingredientId)
    {
        return _ctx.Set<Ingredient>()
            .Include(e => e.IngredientTags)
                .ThenInclude(e => e.Tag)
            .FirstOrDefault(e =>
                e.RestaurantId == restaurantId &&
                e.Id == ingredientId);
    }

    public async Task DeleteIngredient(Ingredient ingredient)
    {
        _ctx.Remove(ingredient);
    }

    public async Task<IngredientTag?> GetTag(Guid restaurantId, short ingredientId, short tagId)
    {
        return await _ctx.FindAsync<IngredientTag>(restaurantId, ingredientId, tagId);
    }

    public async Task AddTag(Ingredient ingredient, Tag tag)
    {
        var ingredientTag = new IngredientTag
        {
            Ingredient = ingredient,
            Tag = tag
        };

        _ctx.Add(ingredientTag);
    }

    public async Task DeleteTag(IngredientTag ingredientTag)
    {
        _ctx.Remove(ingredientTag);
    }
}