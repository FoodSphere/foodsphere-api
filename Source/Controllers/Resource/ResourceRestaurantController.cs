using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FoodSphere.Services;
using FoodSphere.Data.Models;
using FoodSphere.Data.DTOs;

namespace FoodSphere.Controllers.Resource;

public class ResourceRestaurantRequest
{
    public required string owner_id { get; set; }
    public ContactDTO? contact { get; set; }
    public required string name { get; set; }
    public string? display_name { get; set; }
}

public class ResourceRestaurantResponse
{
    public Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public ContactDTO contact { get; set; } = null!;

    public required string name { get; set; }
    public string? display_name { get; set; }

    public static ResourceRestaurantResponse FromModel(Restaurant model)
    {
        return new ResourceRestaurantResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            contact = ContactDTO.FromModel(model.Contact)!,
            name = model.Name,
            display_name = model.DisplayName,
        };
    }
}

[Route("resource/restaurants")]
public class ResourceRestaurantController(
    ILogger<ResourceRestaurantController> logger,
    RestaurantService restaurantService
) : AdminController
{
    readonly ILogger<ResourceRestaurantController> _logger = logger;
    readonly RestaurantService _restaurantService = restaurantService;

    [HttpPost]
    public async Task<ActionResult<ResourceRestaurantResponse>> CreateRestaurant(ResourceRestaurantRequest body)
    {
        var restaurant = await _restaurantService.CreateRestaurant(
            ownerId: body.owner_id,
            name: body.name,
            displayName: body.display_name
        );

        if (body.contact is not null)
        {
            await _restaurantService.SetContact(restaurant, body.contact);
        }

        await _restaurantService.Save();

        return CreatedAtAction(
            nameof(GetRestaurant),
            new { restaurant_id = restaurant.Id },
            ResourceRestaurantResponse.FromModel(restaurant)
        );
    }

    [HttpPost("{restaurant_id}/contact")]
    public async Task<ActionResult<ResourceRestaurantResponse>> SetContact(Guid restaurant_id, ContactDTO body)
    {
        var restaurant = await _restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        await _restaurantService.SetContact(restaurant, body);
        await _restaurantService.Save();

        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("{restaurant_id}")]
    public async Task<ActionResult<ResourceRestaurantResponse>> GetRestaurant(Guid restaurant_id)
    {
        var restaurant = await _restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        return ResourceRestaurantResponse.FromModel(restaurant);
    }

    // [HttpPut("{restaurant_id}")]
    // public async Task<ActionResult> UpdateRestaurant(Guid restaurant_id, RestaurantRequest body)
    // {
    //     var restaurant = await _restaurantService.Get(restaurant_id);

    //     if (restaurant is null)
    //     {
    //         return NotFound();
    //     }

    //     restaurant.Name = body.name;
    //     restaurant.DisplayName = body.display_name;

    //     await _restaurantService.Save();

    //     return NoContent();
    // }

    [HttpDelete("{restaurant_id}")]
    public async Task<ActionResult> DeleteRestaurant(Guid restaurant_id)
    {
        var restaurant = await _restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        await _restaurantService.DeleteRestaurant(restaurant);
        await _restaurantService.Save();

        return NoContent();
    }
}