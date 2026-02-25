using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

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

        // await SeedRole(); ?

        // we don't have to `_ctx.Add(contact)`,
        // because the contact was set in restaurant
        // before the restaurant is added to DbContext
        _ctx.Add(restaurant);

        return restaurant;
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

    public async Task<Restaurant?> GetRestaurant(Guid restaurantId)
    {
        return await _ctx.FindAsync<Restaurant>(restaurantId);
    }

    public async Task<List<Restaurant>> ListRestaurants()
    {
        return await _ctx.Set<Restaurant>().ToListAsync();
    }

    public async Task<List<Restaurant>> ListRestaurants(string ownerId)
    {
        return await _ctx.Set<Restaurant>()
            .Where(restaurant => restaurant.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<RestaurantManager[]> ListManagers(Guid restaurantId)
    {
        return await _ctx.Set<RestaurantManager>()
            .Where(manager => manager.RestaurantId == restaurantId)
            .ToArrayAsync();
    }

    public async Task DeleteRestaurant(Restaurant restaurant)
    {
        _ctx.Remove(restaurant);
        _ctx.Remove(restaurant.Contact);
    }

    public async Task DeleteRestaurant(Guid restaurantId)
    {
        var restaurant = await GetRestaurant(restaurantId);
        await DeleteRestaurant(restaurant!);
    }

    public async Task SetContact(Restaurant restaurant, ContactDto contact)
    {
        restaurant.Contact.Name = contact?.name;
        restaurant.Contact.Email = contact?.email;
        restaurant.Contact.Phone = contact?.phone;
    }

    public async Task SetContact(Guid restaurantId, ContactDto contact)
    {
        var restaurant = await GetRestaurant(restaurantId);
        await SetContact(restaurant!, contact);
    }
}