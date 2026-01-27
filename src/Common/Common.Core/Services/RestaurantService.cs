using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Services;

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
        await _ctx.AddAsync(restaurant, ct);

        return restaurant;
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