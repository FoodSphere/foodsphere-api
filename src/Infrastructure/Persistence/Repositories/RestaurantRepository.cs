namespace FoodSphere.Infrastructure.Repository;

public class RestaurantRepository(
    FoodSphereDbContext context
) : RepositoryBase(context)
{
    public async Task<ResultObject<Restaurant>> CreateRestaurant(
        MasterUserKey ownerKey,
        string name,
        string? displayName,
        Contact? contact,
        CancellationToken ct = default)
    {
        var restaurant = new Restaurant()
        {
            Id = Guid.CreateVersion7(),
            OwnerId = ownerKey.Id,
            Name = name,
            DisplayName = displayName,
            Contact = contact ?? new(),
        };

        _ctx.Add(restaurant);

        return restaurant;
    }

    Restaurant CreateRestaurantStub(RestaurantKey restaurantKey)
    {
        var restaurant = new Restaurant
        {
            Id = restaurantKey.Id,
            OwnerId = default!,
            Name = default!,
        };

        _ctx.Attach(restaurant);

        return restaurant;
    }

    public IQueryable<Restaurant> QueryRestaurants()
    {
        return _ctx.Set<Restaurant>()
            .AsExpandable();
    }

    public IQueryable<Restaurant> QuerySingleRestaurant(RestaurantKey restaurantKey)
    {
        return QueryRestaurants()
            .Where(e =>
                e.Id == restaurantKey.Id);
    }

    public async Task<Restaurant?> GetRestaurant(
        RestaurantKey restaurantKey, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<Restaurant>(restaurantKey, ct);
    }

    public async Task<ResultObject> DeleteRestaurant(
        RestaurantKey key, CancellationToken ct = default)
    {
        var restaurant = await GetRestaurant(key, ct);

        if (restaurant is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(restaurant);

        return ResultObject.Success();
    }
}