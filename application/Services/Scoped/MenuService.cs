using Microsoft.EntityFrameworkCore;
using FoodSphere.Data.Models;

namespace FoodSphere.Services;

public class MenuService(AppDbContext context) : BaseService(context)
{
    public async Task<Menu> CreateMenu(
        Guid restaurantId,
        string name,
        int price,
        string? displayName = null,
        string? description = null,
        string? imageUrl = null,
        CancellationToken cancellationToken = default
    ) {
        var lastId = await _ctx.Set<Menu>()
            .Where(menu => menu.RestaurantId == restaurantId)
            .MaxAsync(menu => (int?)menu.Id, cancellationToken) ?? 0;

        var menu = new Menu
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantId,
            Name = name,
            Price = price,
            DisplayName = displayName,
            Description = description,
            ImageUrl = imageUrl
        };

        await _ctx.AddAsync(menu, cancellationToken);

        return menu;
    }

    public async Task<Ingredient> CreateIngredient(
        Guid restaurantId,
        string name,
        string? description = null,
        string? imageUrl = null,
        string? unit = null,
        CancellationToken cancellationToken = default
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
                .MaxAsync(cancellationToken) ?? 0;
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

        await _ctx.AddAsync(ingredient, cancellationToken);

        return ingredient;
    }

    public async Task<Menu?> GetMenu(Guid restaurantId, short menuId)
    {
        return await _ctx.FindAsync<Menu>(restaurantId, menuId);
    }

    public async Task<Ingredient?> GetIngredient(Guid restaurantId, short ingredientId)
    {
        return await _ctx.FindAsync<Ingredient>(restaurantId, ingredientId);
    }

    public async Task DeleteMenu(Menu menu)
    {
        _ctx.Remove(menu);
    }

    public async Task DeleteIngredient(Ingredient ingredient)
    {
        _ctx.Remove(ingredient);
    }

    async Task<MenuIngredient?> GetMenuIngredient(Guid restaurantId, short menuId, short ingredientId)
    {
        return await _ctx.FindAsync<MenuIngredient>(restaurantId, menuId, ingredientId);
    }

    public async Task UpdateIngredient(Guid restaurantId, short menuId, short ingredientId, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        var item = await GetMenuIngredient(restaurantId, menuId, ingredientId);

        if (item is null)
        {
            if (amount == 0)
            {
                return;
            }
            else
            {
                item = new MenuIngredient
                {
                    RestaurantId = restaurantId,
                    MenuId = menuId,
                    IngredientId = ingredientId,
                    Amount = amount
                };

                _ctx.Add(item);
            }
        }
        else
        {
            if (amount == 0)
            {
                _ctx.Remove(item);
            }
            else
            {
                item.Amount = amount;
                // _ctx.Entry(item).State = EntityState.Modified;
            }
        }
    }
}