using Microsoft.EntityFrameworkCore;
using FoodSphere.Data.Models;
using FoodSphere.Data.DTOs;

namespace FoodSphere.Services;

public class RestaurantService(AppDbContext context) : BaseService(context)
{
    public async Task<Restaurant> CreateRestaurant(
        string ownerId,
        string name,
        string? displayName = null,
        CancellationToken cancellationToken = default
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
        await _ctx.AddAsync(restaurant, cancellationToken);

        return restaurant;
    }

    public async Task<Restaurant?> GetRestaurant(Guid restaurantId)
    {
        return await _ctx.FindAsync<Restaurant>(restaurantId);
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

    public async Task SetContact(Restaurant restaurant, ContactDTO contact)
    {
        restaurant.Contact.Name = contact?.name;
        restaurant.Contact.Email = contact?.email;
        restaurant.Contact.Phone = contact?.phone;
    }

    public async Task SetContact(Guid restaurantId, ContactDTO contact)
    {
        var restaurant = await GetRestaurant(restaurantId);
        await SetContact(restaurant!, contact);
    }
}