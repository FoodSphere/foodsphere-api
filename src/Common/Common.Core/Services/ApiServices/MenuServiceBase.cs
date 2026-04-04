namespace FoodSphere.Common.Service;

public class MenuServiceBase(
    PersistenceService persistenceService,
    MenuRepository menuRepository
) : ServiceBase
{
    public async Task<TDto[]> ListMenus<TDto>(
        Expression<Func<Menu, TDto>> projection,
        Expression<Func<Menu, bool>> predicate,
        CancellationToken ct = default)
    {
        return await menuRepository.QueryMenus()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(MenuKey, TDto)>> CreateMenu<TDto>(
        Expression<Func<Menu, TDto>> projection,
        MenuCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await menuRepository.CreateMenu(
            restaurantKey: command.RestaurantKey,
            name: command.Name,
            price: command.Price,
            displayName: command.DisplayName,
            description: command.Description,
            imageUrl: null,
            ct);

        if (!createResult.TryGetValue(out var menu))
            return createResult.Errors;

        menu.Status = command.Status;

        var tagResult = await menuRepository.SetTags(
            menu, command.Tags, ct);

        if (tagResult.IsFailed)
            return tagResult.Errors;

        var ingredientResult = await menuRepository.SetIngredients(
            menu, command.Ingredients, ct);

        if (ingredientResult.IsFailed)
            return ingredientResult.Errors;

        var componentResult = await menuRepository.SetComponents(
            menu, command.Components, ct);

        if (componentResult.IsFailed)
            return componentResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetMenu(projection, menu, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created menu.");

        return (menu, response);
    }

    public async Task<TDto?> GetMenu<TDto>(
        Expression<Func<Menu, TDto>> projection, MenuKey key,
        CancellationToken ct = default)
    {
        return await menuRepository.QuerySingleMenu(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateMenu(
        MenuKey key, MenuUpdateCommand command,
        CancellationToken ct = default)
    {
        var menu = await menuRepository.GetMenu(key, ct);

        if (menu is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Menu not found.");

        menu.Name = command.Name;
        menu.Price = command.Price;
        menu.DisplayName = command.DisplayName;
        menu.Description = command.Description;
        menu.Status = command.Status;

        var tagResult = await menuRepository.SetTags(
            menu, command.Tags, ct);

        if (tagResult.IsFailed)
            return tagResult.Errors;

        var ingredientResult = await menuRepository.SetIngredients(
            menu, command.Ingredients, ct);

        if (ingredientResult.IsFailed)
            return ingredientResult.Errors;

        var componentResult = await menuRepository.SetComponents(
            menu, command.Components, ct);

        if (componentResult.IsFailed)
            return componentResult.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteMenu(
        MenuKey key, CancellationToken ct = default)
    {
        var result = await menuRepository.DeleteMenu(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> SetTags(
        MenuKey key, IEnumerable<TagKey> tagKeys,
        CancellationToken ct = default)
    {
        var result = await menuRepository.SetTags(
            key, tagKeys, ct);

        if (result.IsFailed)
            return result.Errors;

        return ResultObject.Success();
    }


    public async Task<ResultObject<(MenuTagKey, TDto)?>> AssignTag<TDto>(
        Expression<Func<TagMenu, TDto>> projection,
        MenuKey menuKey, TagKey tagKey,
        CancellationToken ct = default)
    {
        var menuTag = await menuRepository.GetMenuTag(
            new(menuKey.RestaurantId, menuKey.Id, tagKey.Id),
            ct);

        if (menuTag is not null)
            return ResultObject.None();

        var createResult = await menuRepository.CreateMenuTag(
            menuKey, tagKey, ct);

        if (!createResult.TryGetValue(out menuTag))
            return createResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetTagItem(
            projection, menuTag, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created tag.");

        return (menuTag, response);
    }

    public async Task<TDto[]> ListTags<TDto>(
        Expression<Func<TagMenu, TDto>> projection,
        Expression<Func<TagMenu, bool>> predicate,
        CancellationToken ct = default)
    {
        return await menuRepository.QueryMenuTags()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<TDto?> GetTagItem<TDto>(
        Expression<Func<TagMenu, TDto>> projection,
        MenuTagKey key,
        CancellationToken ct = default)
    {
        return await menuRepository.QuerySingleMenuTag(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> DeleteTagItem(
        MenuTagKey key, CancellationToken ct = default)
    {
        var result = await menuRepository.DeleteMenuTag(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject<(MenuIngredientKey, TDto)?>> SetMenuIngredient<TDto>(
        Expression<Func<MenuIngredient, TDto>> projection,
        MenuKey menuKey, IngredientKey ingredientKey, decimal amount,
        CancellationToken ct = default)
    {
        var menuIngredient = await menuRepository.GetMenuIngredient(
            new(menuKey.RestaurantId, menuKey.Id, ingredientKey.Id), ct);

        if (menuIngredient is not null)
        {
            menuIngredient.Amount = amount;
            await persistenceService.Commit(ct);

            return ResultObject.None();
        }

        var createResult = await menuRepository.CreateMenuIngredient(
            menuKey, ingredientKey, amount, ct);

        if (!createResult.TryGetValue(out menuIngredient))
            return createResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetMenuIngredient(
            projection, menuIngredient, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created menu's ingredient.");

        return (menuIngredient, response);
    }

    public async Task<ResultObject> SetIngredients(
        MenuKey key, IEnumerable<(IngredientKey, decimal)> ingredients,
        CancellationToken ct = default)
    {
        var result = await menuRepository.SetIngredients(
            key, ingredients, ct);

        if (result.IsFailed)
            return result.Errors;

        return ResultObject.Success();
    }

    public async Task<TDto[]> ListMenuIngredients<TDto>(
        Expression<Func<MenuIngredient, TDto>> projection,
        Expression<Func<MenuIngredient, bool>> predicate,
        CancellationToken ct = default)
    {
        return await menuRepository.QueryMenuIngredients()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<TDto?> GetMenuIngredient<TDto>(
        Expression<Func<MenuIngredient, TDto>> projection,
        MenuIngredientKey menuIngredientKey,
        CancellationToken ct = default)
    {
        return await menuRepository.QuerySingleMenuIngredient(menuIngredientKey)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> DeleteMenuIngredient(
        MenuIngredientKey key, CancellationToken ct = default)
    {
        var result = await menuRepository.DeleteMenuIngredient(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject<(MenuComponentKey, TDto)?>> SetMenuComponent<TDto>(
        Expression<Func<MenuComponent, TDto>> projection,
        MenuKey parentKey, MenuKey childKey, short quantity,
        CancellationToken ct = default)
    {
        var menuComponent = await menuRepository.GetMenuComponent(
            new(parentKey.RestaurantId, parentKey.Id, childKey.Id), ct);

        if (menuComponent is not null)
        {
            menuComponent.Quantity = quantity;
            await persistenceService.Commit(ct);

            return ResultObject.None();
        }

        var createResult = await menuRepository.CreateMenuComponent(
            parentKey, childKey, quantity, ct);

        if (!createResult.TryGetValue(out menuComponent))
            return createResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetMenuComponent(
            projection, menuComponent, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created menu's component.");

        return (menuComponent, response);
    }

    public async Task<ResultObject> SetComponents(
        MenuKey key, IEnumerable<(MenuKey, short)> components,
        CancellationToken ct = default)
    {
        var result = await menuRepository.SetComponents(
            key, components, ct);

        if (result.IsFailed)
            return result.Errors;

        return ResultObject.Success();
    }

    public async Task<TDto[]> ListMenuComponents<TDto>(
        Expression<Func<MenuComponent, TDto>> projection,
        Expression<Func<MenuComponent, bool>> predicate,
        CancellationToken ct = default)
    {
        return await menuRepository.QueryMenuComponents()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<TDto?> GetMenuComponent<TDto>(
        Expression<Func<MenuComponent, TDto>> projection,
        MenuComponentKey menuComponentKey,
        CancellationToken ct = default)
    {
        return await menuRepository.QuerySingleMenuComponent(menuComponentKey)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> DeleteMenuComponent(
        MenuComponentKey key, CancellationToken ct = default)
    {
        var result = await menuRepository.DeleteMenuComponent(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}