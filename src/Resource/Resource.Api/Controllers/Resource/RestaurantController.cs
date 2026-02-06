namespace FoodSphere.Resource.Api.Controller;

[Route("restaurants")]
public class RestaurantController(
    ILogger<RestaurantController> logger,
    RestaurantService restaurantService
) : ResourceControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RestaurantResponse>>> ListRestaurants()
    {
        var restaurants = await restaurantService.ListRestaurants();

        return restaurants.Select(RestaurantResponse.FromModel).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<RestaurantResponse>> CreateRestaurant(RestaurantRequest body)
    {
        var restaurant = await restaurantService.CreateRestaurant(
            ownerId: body.owner_id,
            name: body.name,
            displayName: body.display_name
        );

        if (body.contact is not null)
        {
            await restaurantService.SetContact(restaurant, body.contact);
        }

        await restaurantService.SaveAsync();

        return CreatedAtAction(
            nameof(GetRestaurant),
            new { restaurant_id = restaurant.Id },
            RestaurantResponse.FromModel(restaurant)
        );
    }

    [HttpGet("{restaurant_id}")]
    public async Task<ActionResult<RestaurantResponse>> GetRestaurant(Guid restaurant_id)
    {
        var restaurant = await restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        return RestaurantResponse.FromModel(restaurant);
    }

    [HttpPost("{restaurant_id}/contact")]
    public async Task<ActionResult<RestaurantResponse>> SetContact(Guid restaurant_id, ContactDto body)
    {
        var restaurant = await restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        await restaurantService.SetContact(restaurant, body);
        await restaurantService.SaveAsync();

        return NoContent();
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
        var restaurant = await restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        await restaurantService.DeleteRestaurant(restaurant);
        await restaurantService.SaveAsync();

        return NoContent();
    }
}