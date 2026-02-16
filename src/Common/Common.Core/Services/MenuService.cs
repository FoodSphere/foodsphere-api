namespace FoodSphere.Common.Service;

public class MenuService(
    FoodSphereDbContext context
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
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.RestaurantId == restaurantId);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Menu>().Local
                .Where(e => e.RestaurantId == restaurantId)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Menu>()
                .Where(e => e.RestaurantId == restaurantId)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
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

    public Menu GetMenuStub(Guid restaurantId, short menuId)
    {
        var menu = new Menu
        {
            Id = menuId,
            RestaurantId = restaurantId,
            Name = default!,
        };

        _ctx.Attach(menu);

        return menu;
    }

    public IQueryable<Menu> QueryMenus()
    {
        return _ctx.Set<Menu>()
            .AsExpandable();
    }

    public IQueryable<Menu> QuerySingleMenu(Guid restaurantId, short menuId)
    {
        return QueryMenus()
            .Where(e =>
                e.RestaurantId == restaurantId &&
                e.Id == menuId);
    }

    public async Task<TDto?> GetMenu<TDto>(Guid restaurantId, short menuId, Expression<Func<Menu, TDto>> projection)
    {
        return await QuerySingleMenu(restaurantId, menuId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<Menu?> GetMenu(Guid restaurantId, short menuId)
    {
        var existed = await _ctx.Set<Menu>()
            .AnyAsync(e =>
                e.RestaurantId == restaurantId &&
                e.Id == menuId);

        if (!existed)
        {
            return null;
        }

        return GetMenuStub(restaurantId, menuId);
    }

    public async Task DeleteMenu(Menu menu)
    {
        _ctx.Remove(menu);
    }

    public async Task<MenuTag> CreateMenuTag(Menu menu, short tagId)
    {
        var menuTag = new MenuTag
        {
            Menu = menu,
            TagId = tagId
        };

        _ctx.Add(menuTag);

        return menuTag;
    }

    public MenuTag GetMenuTagStub(Guid restaurantId, short menuId, short tagId)
    {
        var menuTag = new MenuTag
        {
            RestaurantId = restaurantId,
            MenuId = menuId,
            TagId = tagId
        };

        _ctx.Attach(menuTag);

        return menuTag;
    }

    public IQueryable<MenuTag> QueryMenuTags(Guid restaurantId, short menuId)
    {
        return _ctx.Set<MenuTag>()
            .AsExpandable()
            .Where(e =>
                e.RestaurantId == restaurantId &&
                e.MenuId == menuId);
    }

    public IQueryable<MenuTag> QueryMenuTags(Menu menu)
    {
        return QueryMenuTags(menu.RestaurantId, menu.Id);
    }

    public IQueryable<MenuTag> QuerySingleMenuTag(Guid restaurantId, short menuId, short tagId)
    {
        return QueryMenuTags(restaurantId, menuId)
            .Where(e => e.TagId == tagId);
    }

    public async Task<TDto?> GetMenuTag<TDto>(Guid restaurantId, short menuId, short tagId, Expression<Func<MenuTag, TDto>> projection)
    {
        return await QuerySingleMenuTag(restaurantId, menuId, tagId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<MenuTag?> GetMenuTag(Guid restaurantId, short menuId, short tagId)
    {
        var existed = await _ctx.Set<MenuTag>()
            .AnyAsync(e =>
                e.RestaurantId == restaurantId &&
                e.MenuId == menuId &&
                e.TagId == tagId);

        if (!existed)
        {
            return null;
        }

        return GetMenuTagStub(restaurantId, menuId, tagId);
    }

    public async Task DeleteMenuTag(MenuTag menuTag)
    {
        _ctx.Remove(menuTag);
    }

    public async Task<MenuIngredient> AssignIngredient(Menu menu, short ingredientId, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0);

        var menuIngredient = new MenuIngredient
        {
            Menu = menu,
            IngredientId = ingredientId,
            Amount = amount
        };

        _ctx.Add(menuIngredient);

        return menuIngredient;
    }

    public MenuIngredient GetMenuIngredientStub(Guid restaurantId, short menuId, short ingredientId)
    {
        var menuIngredient = new MenuIngredient
        {
            RestaurantId = restaurantId,
            MenuId = menuId,
            IngredientId = ingredientId
        };

        _ctx.Attach(menuIngredient);

        return menuIngredient;
    }

    public IQueryable<MenuIngredient> QueryMenuIngredients(Guid restaurantId, short menuId)
    {
        return _ctx.Set<MenuIngredient>()
            .AsExpandable()
            .Where(e =>
                e.RestaurantId == restaurantId &&
                e.MenuId == menuId);
    }

    public IQueryable<MenuIngredient> QueryMenuIngredients(Menu menu)
    {
        return QueryMenuIngredients(menu.RestaurantId, menu.Id);
    }

    public IQueryable<MenuIngredient> QuerySingleMenuIngredient(Guid restaurantId, short menuId, short ingredientId)
    {
        return QueryMenuIngredients(restaurantId, menuId)
            .Where(e => e.IngredientId == ingredientId);
    }

    public async Task<TDto?> GetMenuIngredient<TDto>(Guid restaurantId, short menuId, short ingredientId, Expression<Func<MenuIngredient, TDto>> projection)
    {
        return await QuerySingleMenuIngredient(restaurantId, menuId, ingredientId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<MenuIngredient?> GetMenuIngredient(Guid restaurantId, short menuId, short ingredientId)
    {
        var existed = await _ctx.Set<MenuIngredient>()
            .AnyAsync(e =>
                e.RestaurantId == restaurantId &&
                e.MenuId == menuId &&
                e.IngredientId == ingredientId);

        if (!existed)
        {
            return null;
        }

        return GetMenuIngredientStub(restaurantId, menuId, ingredientId);
    }

    public async Task DeleteMenuIngredient(MenuIngredient menuIngredient)
    {
        _ctx.Remove(menuIngredient);
    }
}