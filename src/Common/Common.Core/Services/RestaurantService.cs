namespace FoodSphere.Common.Service;

public class RestaurantService(FoodSphereDbContext context) : ServiceBase(context)
{
    public async Task<Restaurant> CreateRestaurant(
        string ownerId,
        string name,
        string? displayName = null,
        CancellationToken ct = default
    ) {
        var restaurant = new Restaurant()
        {
            OwnerId = ownerId,
            Contact = new Contact(),
            Name = name,
            DisplayName = displayName,
        };

        // we don't have to `_ctx.Add(contact)`,
        // because the contact was set in restaurant
        // before the restaurant is added to DbContext
        _ctx.Add(restaurant);

        return restaurant;
    }

    public Restaurant GetRestaurantStub(Guid restaurantId)
    {
        var restaurant = new Restaurant
        {
            Id = restaurantId,
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

    public IQueryable<Restaurant> QuerySingleRestaurant(Guid restaurantId)
    {
        return QueryRestaurants()
            .Where(e =>
                e.Id == restaurantId);
    }

    public async Task<TDto?> GetRestaurant<TDto>(Guid restaurantId, Expression<Func<Restaurant, TDto>> projection)
    {
        return await QuerySingleRestaurant(restaurantId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<Restaurant?> GetRestaurant(Guid restaurantId)
    {
        var existed = await _ctx.Set<Restaurant>()
            .AnyAsync(e =>
                e.Id == restaurantId);

        if (!existed)
        {
            return null;
        }

        return GetRestaurantStub(restaurantId);
    }

    public async Task DeleteRestaurant(Restaurant restaurant)
    {
        _ctx.Remove(restaurant);
    }

    public async Task SetContact(Restaurant restaurant, ContactDto contact)
    {
        restaurant.Contact.Name = contact.name;
        restaurant.Contact.Email = contact.email;
        restaurant.Contact.Phone = contact.phone;
    }

    public async Task SetContact(Guid restaurantId, ContactDto contact)
    {
        var restaurant = await GetRestaurant(restaurantId);
        await SetContact(restaurant!, contact);
    }

    public async Task<RestaurantManager> CreateManager(
        Guid restaurantId,
        string masterId,
        CancellationToken ct = default
    ) {
        var manager = new RestaurantManager
        {
            RestaurantId = restaurantId,
            MasterId = masterId,
        };

        _ctx.Add(manager);

        return manager;
    }

    public RestaurantManager GetManagerStub(Guid restaurantId, string masterId)
    {
        var manager = new RestaurantManager
        {
            RestaurantId = restaurantId,
            MasterId = masterId,
        };

        _ctx.Attach(manager);

        return manager;
    }

    public IQueryable<RestaurantManager> QueryManagers()
    {
        return _ctx.Set<RestaurantManager>()
            .AsExpandable();
    }

    public IQueryable<RestaurantManager> QuerySingleManager(Guid restaurantId, string masterId)
    {
        return QueryManagers()
            .Where(e =>
                e.RestaurantId == restaurantId &&
                e.MasterId == masterId);
    }

    public async Task<RestaurantManager?> GetManager(Guid restaurantId, string masterId)
    {
        var existed = await _ctx.Set<RestaurantManager>()
            .AnyAsync(e =>
                e.RestaurantId == restaurantId &&
                e.MasterId == masterId);

        if (!existed)
        {
            return null;
        }

        return GetManagerStub(restaurantId, masterId);
    }

    public async Task<TDto?> GetManager<TDto>(Guid restaurantId, string masterId, Expression<Func<RestaurantManager, TDto>> projection)
    {
        return await QuerySingleManager(restaurantId, masterId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task DeleteManager(RestaurantManager manager)
    {
        _ctx.Remove(manager);
    }

    public async Task SetManagerRoles(
        RestaurantManager manager,
        IEnumerable<short> roleIds,
        CancellationToken ct = default
    ) {
        await SetManagerRoles(
            manager.RestaurantId,
            manager.MasterId,
            roleIds,
            ct);
    }

    public async Task SetManagerRoles(
        Guid restaurantId,
        string masterId,
        IEnumerable<short> roleIds,
        CancellationToken ct = default
    ) {
        var desiredIds = roleIds
            .Distinct()
            .ToArray();

        var currentRoles = await _ctx.Set<RestaurantManagerRole>()
            .Where(rmr =>
                rmr.RestaurantId == restaurantId &&
                rmr.ManagerId == masterId)
            .ToArrayAsync(ct);

        var toRemove = currentRoles
            .ExceptBy(desiredIds, sr => sr.RoleId)
            .ToArray();

        var toAddIds = desiredIds
            .Except(currentRoles.Select(sr => sr.RoleId))
            .ToArray();

        var newEntities = toAddIds.Select(roleId => new RestaurantManagerRole
        {
            RestaurantId = restaurantId,
            ManagerId = masterId,
            RoleId = roleId
        });

        _ctx.RemoveRange(toRemove);
        await _ctx.AddRangeAsync(newEntities, ct);
    }
}