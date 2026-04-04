namespace FoodSphere.Common.Service;

public class RestaurantServiceBase(
    PersistenceService persistenceService,
    RestaurantRepository restaurantRepository
) : ServiceBase
{
    public async Task<TDto[]> ListRestaurants<TDto>(
        Expression<Func<Restaurant, TDto>> projection,
        Expression<Func<Restaurant, bool>> predicate,
        CancellationToken ct = default)
    {
        return await restaurantRepository.QueryRestaurants()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(RestaurantKey, TDto)>> CreateRestaurant<TDto>(
        Expression<Func<Restaurant, TDto>> projection,
        RestaurantCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await restaurantRepository.CreateRestaurant(
            ownerKey: command.OwnerKey,
            name: command.Name,
            displayName: command.DisplayName,
            contact: command.Contact,
            ct);

        if (!createResult.TryGetValue(out var restaurant))
            return createResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetRestaurant(projection, restaurant, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created restaurant.");

        return (restaurant, response);
    }

    public async Task<TDto?> GetRestaurant<TDto>(
        Expression<Func<Restaurant, TDto>> projection, RestaurantKey key,
        CancellationToken ct = default)
    {
        return await restaurantRepository.QuerySingleRestaurant(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateRestaurant(
        RestaurantKey key, RestaurantUpdateCommand command,
        CancellationToken ct = default)
    {
        var restaurant = await restaurantRepository.GetRestaurant(key, ct);

        if (restaurant is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Restaurant not found.");

        restaurant.Name = command.Name;
        restaurant.DisplayName = command.DisplayName;
        restaurant.Contact.Email = command.Contact?.Email;
        restaurant.Contact.Name = command.Contact?.Name;
        restaurant.Contact.Phone = command.Contact?.Phone;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteRestaurant(
        RestaurantKey key, CancellationToken ct = default)
    {
        var result = await restaurantRepository.DeleteRestaurant(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}