namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants")]
public class RestaurantController(
    ILogger<RestaurantController> logger,
    RestaurantService restaurantService
) : MasterControllerBase
{
    /// <summary>
    /// create restaurant
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RestaurantResponse>> CreateRestaurant(RestaurantRequest body)
    {
        var restaurant = await restaurantService.CreateRestaurant(
            ownerId: MasterId,
            name: body.name,
            displayName: body.display_name
        );

        if (body.contact is not null)
        {
            await restaurantService.SetContact(restaurant, body.contact);
        }

        await restaurantService.SaveChanges();

        return CreatedAtAction(
            nameof(InfoController.GetRestaurant),
            GetControllerName(nameof(InfoController)),
            new { restaurant_id = restaurant.Id },
            RestaurantResponse.FromModel(restaurant)
        );
    }

    /// <summary>
    /// list owned restaurants
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<RestaurantResponse[]>> ListMyRestaurants()
    {
        var restaurant = await restaurantService.ListRestaurants(MasterId);

        return restaurant
            .Select(RestaurantResponse.FromModel)
            .ToArray();
    }

    /// <summary>
    /// get restaurant's contact
    /// </summary>
    [HttpGet("{restaurant_id}/contact")]
    public async Task<ActionResult<ContactDto>> GetContact(Guid restaurant_id)
    {
        var restaurant = await restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        if (restaurant.OwnerId != MasterId)
        {
            return Forbid();
        }

        return ContactDto.FromModel(restaurant.Contact)!;
    }

    /// <summary>
    /// set restaurant's contact
    /// </summary>
    [HttpPut("{restaurant_id}/contact")]
    public async Task<ActionResult> SetContact(Guid restaurant_id, ContactDto body)
    {
        var restaurant = await restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        if (restaurant.OwnerId != MasterId)
        {
            return Forbid();
        }

        await restaurantService.SetContact(restaurant, body);
        await restaurantService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// delete restaurant
    /// </summary>
    [HttpDelete("{restaurant_id}")]
    public async Task<ActionResult> DeleteRestaurant(Guid restaurant_id)
    {
        var restaurant = await restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        if (restaurant.OwnerId != MasterId)
        {
            return Forbid();
        }

        await restaurantService.DeleteRestaurant(restaurant);
        await restaurantService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// create restaurant's manager
    /// </summary>
    [HttpPost("{restaurant_id}/managers")]
    public async Task<ActionResult<RestaurantManagerResponse>> CreateManager(Guid restaurant_id, ManagerRequest body)
    {
        var manager = await restaurantService.CreateManager(restaurant_id, body.master_id);

        await restaurantService.SaveChanges();

        return RestaurantManagerResponse.FromModel(manager);
    }

    /// <summary>
    /// list restaurant's managers
    /// </summary>
    [HttpGet("{restaurant_id}/managers")]
    public async Task<ActionResult<RestaurantManagerResponse[]>> ListManagers(Guid restaurant_id)
    {
        var managers = await restaurantService.ListManagers(restaurant_id);

        return managers
            .Select(RestaurantManagerResponse.FromModel)
            .ToArray();
    }
}