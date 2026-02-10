using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Service;

public class MenuUpdateService(
    FoodSphereDbContext context,
    IStorageService storageService
) : ServiceBase(context)
{
    public async Task<Menu> CreateMenu(
        Guid restaurantId,
        string name,
        int price,
        string? displayName = null,
        string? description = null,
        string? imageUrl = null,
        CancellationToken ct = default
    ) {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Menu>()
            .Any(e => e.State == EntityState.Added && e.Entity.RestaurantId == restaurantId);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Menu>().Local
                .Where(menu => menu.RestaurantId == restaurantId)
                .Max(menu => (int?)menu.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Menu>()
                .Where(menu => menu.RestaurantId == restaurantId)
                .Select(menu => (int?)menu.Id)
                .MaxAsync(ct) ?? 0;
        }

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

        _ctx.Add(menu);

        return menu;
    }

    public async Task<List<Menu>> ListMenus(Guid restaurantId)
    {
        return await _ctx.Set<Menu>()
            .Where(menu => menu.RestaurantId == restaurantId)
            .Include(e => e.MenuIngredients)
            .Include(e => e.MenuTags)
                .ThenInclude(e => e.Tag)
            .ToListAsync();
    }

    public async Task<Menu?> FindMenu(Guid restaurantId, short menuId)
    {
        return await _ctx.FindAsync<Menu>(restaurantId, menuId);
    }

    public async Task<Menu?> GetMenu(Guid restaurantId, short menuId)
    {
        return _ctx.Set<Menu>()
            .Include(e => e.MenuIngredients)
            .Include(e => e.Components)
            .Include(e => e.MenuTags)
                .ThenInclude(e => e.Tag)
            .FirstOrDefault(e =>
                e.RestaurantId == restaurantId &&
                e.Id == menuId);
    }

    public async Task<string?> UploadImage(Menu menu, Stream fileStream, string contentType)
    {
        var result = await storageService.Upload("public", "menu", fileStream, contentType);

        if (result.Successed)
        {
            menu.ImageUrl = result.ObjectUrl;

            return result.ObjectUrl;
        }

        return null;
    }

    public async Task<string> GetImageUploadUrl(Menu menu)
    {
        var result = await storageService.GetUploadPreSignedUrl("public", "menu");

        menu.ImageUrl = result.ObjectUrl;

        return result.Url;
    }

    public async Task DeleteMenu(Menu menu)
    {
        _ctx.Remove(menu);
    }

    public async Task<MenuTag?> GetTag(Guid restaurantId, short menuId, short tagId)
    {
        return await _ctx.FindAsync<MenuTag>(restaurantId, menuId, tagId);
    }

    public async Task AddTag(Menu menu, Tag tag)
    {
        var menuTag = new MenuTag
        {
            Menu = menu,
            Tag = tag
        };

        _ctx.Add(menuTag);
    }

    public async Task DeleteTag(MenuTag menuTag)
    {
        _ctx.Remove(menuTag);
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