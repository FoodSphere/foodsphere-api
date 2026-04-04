namespace FoodSphere.Infrastructure.Repository;

public class MenuRepository(
    FoodSphereDbContext context
) : RepositoryBase(context)
{
    public async Task<ResultObject<Menu>> CreateMenu(
        RestaurantKey restaurantKey,
        string name,
        int price,
        string? displayName,
        string? description,
        string? imageUrl,
        CancellationToken ct = default)
    {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Menu>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.RestaurantId == restaurantKey.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Menu>().Local
                .Where(e => e.RestaurantId == restaurantKey.Id)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Menu>()
                .Where(e => e.RestaurantId == restaurantKey.Id)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var menu = new Menu
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantKey.Id,
            Name = name,
            Price = price,
            DisplayName = displayName,
            Description = description,
            ImageUrl = imageUrl
        };

        _ctx.Add(menu);

        return menu;
    }

    public IQueryable<Menu> QueryMenus()
    {
        return _ctx.Set<Menu>()
            .AsExpandable();
    }

    public IQueryable<Menu> QuerySingleMenu(MenuKey key)
    {
        return QueryMenus()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.Id == key.Id);
    }

    public async Task<Menu?> GetMenu(
        MenuKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<Menu>(key, ct);
    }

    public async Task<ResultObject> DeleteMenu(
        MenuKey key, CancellationToken ct = default)
    {
        var menu = await GetMenu(key, ct);

        if (menu is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(menu);

        return ResultObject.Success();
    }

    public async Task<ResultObject<TagMenu>> CreateMenuTag(
        MenuKey menuKey, TagKey tagKey,
        CancellationToken ct = default)
    {
        var menuTag = new TagMenu
        {
            RestaurantId = menuKey.RestaurantId,
            MenuId = menuKey.Id,
            TagId = tagKey.Id,
        };

        _ctx.Add(menuTag);

        return menuTag;
    }

    public IQueryable<TagMenu> QueryMenuTags()
    {
        return _ctx.Set<TagMenu>()
            .AsExpandable();
    }

    public IQueryable<TagMenu> QueryMenuTags(MenuKey key)
    {
        return QueryMenuTags()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.MenuId == key.Id);
    }

    public IQueryable<TagMenu> QuerySingleMenuTag(MenuTagKey key)
    {
        return QueryMenuTags()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.MenuId == key.MenuId &&
                e.TagId == key.TagId);
    }

    public async Task<TagMenu?> GetMenuTag(
        MenuTagKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<TagMenu>(key, ct);
    }

    public async Task<ResultObject> DeleteMenuTag(
        MenuTagKey key, CancellationToken ct = default)
    {
        var menuTag = await GetMenuTag(key, ct);

        if (menuTag is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(menuTag);

        return ResultObject.Success();
    }

    public async Task<ResultObject> SetTags(
        MenuKey menuKey,
        IEnumerable<TagKey> tagKeys,
        CancellationToken ct = default)
    {
        var desired = tagKeys
            .Distinct()
            .ToArray();

        var existed = await QueryMenuTags(menuKey)
            .ToArrayAsync(ct);

        var toRemove = existed
            .ExceptBy(desired.Select(e => e.Id), e => e.TagId);

        var toAdd = desired
            .ExceptBy(existed.Select(e => e.TagId), e => e.Id);

        foreach (var tagKey in toAdd)
            await CreateMenuTag(menuKey, tagKey, ct);

        foreach (var tag in toRemove)
            await DeleteMenuTag(tag, ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject<MenuIngredient>> CreateMenuIngredient(
        MenuKey menuKey, IngredientKey ingredientKey, decimal amount,
        CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(
            menuKey.RestaurantId, ingredientKey.RestaurantId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        var menuIngredient = new MenuIngredient
        {
            RestaurantId = menuKey.RestaurantId,
            MenuId = menuKey.Id,
            IngredientId = ingredientKey.Id,
            Amount = amount
        };

        _ctx.Add(menuIngredient);

        return menuIngredient;
    }

    public IQueryable<MenuIngredient> QueryMenuIngredients()
    {
        return _ctx.Set<MenuIngredient>()
            .AsExpandable();
    }

    public IQueryable<MenuIngredient> QueryMenuIngredients(MenuKey key)
    {
        return QueryMenuIngredients()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.MenuId == key.Id);
    }

    public IQueryable<MenuIngredient> QuerySingleMenuIngredient(
        MenuIngredientKey key)
    {
        return QueryMenuIngredients()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.MenuId == key.MenuId &&
                e.IngredientId == key.IngredientId);
    }

    public async Task<MenuIngredient?> GetMenuIngredient(
        MenuIngredientKey menuIngredientKey,
        CancellationToken ct = default)
    {
        return await _ctx.FindAsync<MenuIngredient>(menuIngredientKey, ct);
    }

    public async Task<ResultObject> SetIngredients(
        MenuKey menuKey,
        IEnumerable<(IngredientKey, decimal)> ingredients,
        CancellationToken ct = default)
    {
        var desired = ingredients
            .GroupBy(e => e.Item1)
            .ToDictionary(
                g => g.Key,
                g => g.Last().Item2);

        var existed = await QueryMenuIngredients(menuKey)
            .ToDictionaryAsync(e => e.IngredientId, e => e, ct);

        foreach (var (ingredientKey, amount) in desired)
        {
            if (!existed.TryGetValue(ingredientKey.Id, out var menuIngredient))
            {
                var createResult = await CreateMenuIngredient(
                    menuKey, ingredientKey, amount, ct);

                if (createResult.IsFailed)
                    return createResult.Errors;
            }
            else if (menuIngredient.Amount != amount)
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

                menuIngredient.Amount = amount;
            }
        }

        var toRemove = existed
            .ExceptBy(desired.Keys.Select(e => e.Id), e => e.Key);

        foreach (var (_, menuIngredient) in toRemove)
        {
            var deleteResult = await DeleteMenuIngredient(menuIngredient, ct);

            if (deleteResult.IsFailed)
                return deleteResult.Errors;
        }

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteMenuIngredient(
        MenuIngredientKey key, CancellationToken ct = default)
    {
        var menuIngredient = await GetMenuIngredient(key, ct);

        if (menuIngredient is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(menuIngredient);

        return ResultObject.Success();
    }

    async Task<ResultObject> PrefetchMenuComponents(
        IEnumerable<MenuKey> menuKeys,
        CancellationToken ct = default)
    {
        var restaurantIds = menuKeys
            .Select(i => i.RestaurantId)
            .Distinct()
            .ToArray();

        if (restaurantIds.Length == 0)
            return ResultObject.Success();

        if (restaurantIds.Length > 1)
            return ResultObject.Fail(ResultError.Argument,
                "All menu keys must belong to the same restaurant.",
                restaurantIds);

        var menuIds = menuKeys.Select(i => i.Id)
            .ToHashSet();

        var queriedMenu = await QueryMenus()
            .Where(e =>
                e.RestaurantId == restaurantIds[0] &&
                menuIds.Contains(e.Id))
            .Include(e => e.Components)
            .ToArrayAsync(ct);

        var missingIds = menuIds.Except(
            queriedMenu.Select(e => e.Id));

        if (missingIds.Any())
            return ResultObject.Fail(ResultError.NotFound,
                "Menus not found", new { menu_ids = missingIds });

        return ResultObject.Success();
    }

    public async Task<ResultObject<MenuComponent>> CreateMenuComponent(
        MenuKey parentKey, MenuKey childKey, short quantity,
        CancellationToken ct = default)
    {
        if (parentKey.RestaurantId != childKey.RestaurantId)
            return ResultObject.Fail(ResultError.Argument,
                "Parent menu and child menu must belong to the same restaurant.");

        if (quantity <= 0)
            return ResultObject.Fail(ResultError.Argument,
                "Quantity must be a positive integer.");

        if (parentKey.Id == childKey.Id)
            return ResultObject.Fail(ResultError.Argument,
                "A menu cannot be a component of itself.");

        var childMenu = await GetMenu(childKey, ct);

        if (childMenu is null)
            return ResultObject.NotFound(childKey);

        var collectionEntry = _ctx.Entry(childMenu).Collection(b => b.Components);

        if (!collectionEntry.IsLoaded)
            await collectionEntry.LoadAsync(ct);

        if (childMenu.Components.Count != 0)
            return ResultObject.Fail(ResultError.State,
                "A menu that has components cannot be a component of another menu.");

        var menuComponent = new MenuComponent
        {
            RestaurantId = parentKey.RestaurantId,
            ParentMenuId = parentKey.Id,
            ChildMenuId = childKey.Id,
            Quantity = quantity
        };

        _ctx.Add(menuComponent);

        return menuComponent;
    }

    public IQueryable<MenuComponent> QueryMenuComponents()
    {
        return _ctx.Set<MenuComponent>()
            .AsExpandable();
    }

    public IQueryable<MenuComponent> QueryMenuComponents(MenuKey key)
    {
        return QueryMenuComponents()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.ParentMenuId == key.Id);
    }

    public IQueryable<MenuComponent> QuerySingleMenuComponent(
        MenuComponentKey key)
    {
        return QueryMenuComponents()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.ParentMenuId == key.ParentMenuId &&
                e.ChildMenuId == key.ChildMenuId);
    }

    public async Task<MenuComponent?> GetMenuComponent(
        MenuComponentKey menuComponentKey,
        CancellationToken ct = default)
    {
        return await _ctx.FindAsync<MenuComponent>(menuComponentKey, ct);
    }

    public async Task<ResultObject> SetComponents(
        MenuKey menuKey,
        IEnumerable<(MenuKey, short)> components,
        CancellationToken ct = default)
    {
        var desired = components
            .GroupBy(e => e.Item1)
            .ToDictionary(
                g => g.Key,
                g => g.Last().Item2);

        var existed = await QueryMenuComponents(menuKey)
            .ToDictionaryAsync(e => e.ChildMenuId, e => e, ct);

        await PrefetchMenuComponents(
            desired.Keys.ExceptBy(existed.Keys, menuKey => menuKey.Id), ct);

        foreach (var (componentKey, quantity) in desired)
        {
            if (!existed.TryGetValue(componentKey.Id, out var menuComponent))
            {
                var createResult = await CreateMenuComponent(
                    menuKey, componentKey, quantity, ct);

                if (createResult.IsFailed)
                    return createResult.Errors;
            }
            else if (menuComponent.Quantity != quantity)
            {
                if (quantity <= 0)
                    return ResultObject.Fail(ResultError.Argument,
                        "Quantity must be a positive integer.");

                menuComponent.Quantity = quantity;
            }
        }

        var toRemove = existed
            .ExceptBy(desired.Keys.Select(e => e.Id), e => e.Key);

        foreach (var (_, menuComponent) in toRemove)
        {
            var deleteResult = await DeleteMenuComponent(menuComponent, ct);

            if (deleteResult.IsFailed)
                return deleteResult.Errors;
        }

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteMenuComponent(
        MenuComponentKey key, CancellationToken ct = default)
    {
        var menuComponent = await GetMenuComponent(key, ct);

        if (menuComponent is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(menuComponent);

        return ResultObject.Success();
    }
}