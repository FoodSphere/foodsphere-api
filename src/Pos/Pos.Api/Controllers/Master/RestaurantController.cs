namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants")]
public class RestaurantController(
    ILogger<RestaurantController> logger,
    RestaurantService restaurantService
) : MasterControllerBase
{
    /// <summary>
    /// list owned restaurants
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<RestaurantResponse>>> ListOwnedRestaurants()
    {
        var responses = await restaurantService.QueryRestaurants()
            .Where(e => e.OwnerId == MasterId)
            .Select(RestaurantResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

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

        var response = await restaurantService.GetRestaurant(
            restaurant.Id, RestaurantResponse.Projection);

        return CreatedAtAction(
            nameof(InfoController.GetRestaurant),
            GetControllerName(nameof(InfoController)),
            new { restaurant_id = restaurant.Id },
            response);
    }

    /// <summary>
    /// update restaurant
    /// </summary>
    [HttpPut("{restaurant_id}")]
    public async Task<ActionResult> UpdateRestaurant(Guid restaurant_id, RestaurantRequest body)
    {
        var restaurant = restaurantService.GetRestaurantStub(restaurant_id);

        // if (restaurant.OwnerId != MasterId)
        // {
        //     return Forbid();
        // }

        restaurant.Name = body.name;
        restaurant.DisplayName = body.display_name;

        if (body.contact is not null)
        {
            await restaurantService.SetContact(restaurant, body.contact);
        }

        var affected = await restaurantService.SaveChanges();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// delete restaurant
    /// </summary>
    [HttpDelete("{restaurant_id}")]
    public async Task<ActionResult> DeleteRestaurant(Guid restaurant_id)
    {
        var restaurant = restaurantService.GetRestaurantStub(restaurant_id);

        // if (restaurant.OwnerId != MasterId)
        // {
        //     return Forbid();
        // }

        await restaurantService.DeleteRestaurant(restaurant);

        var affected = await restaurantService.SaveChanges();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// list restaurant's managers
    /// </summary>
    [HttpGet("{restaurant_id}/managers")]
    public async Task<ActionResult<ICollection<RestaurantManagerResponse>>> ListManagers(Guid restaurant_id)
    {
        var responses = await restaurantService
            .QueryManagers()
            .Where(e =>
                e.RestaurantId == restaurant_id)
            .Select(RestaurantManagerResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create restaurant's manager
    /// </summary>
    [HttpPost("{restaurant_id}/managers")]
    public async Task<ActionResult<RestaurantManagerResponse>> CreateManager(Guid restaurant_id, ManagerRequest body)
    {
        var manager = await restaurantService.CreateManager(
            restaurantId: restaurant_id,
            masterId: body.master_id
        );

        await restaurantService.SetManagerRoles(manager, body.roles);

        await restaurantService.SaveChanges();

        var response = await restaurantService.GetManager(
            restaurant_id, manager.MasterId,
            RestaurantManagerResponse.Projection);

        return CreatedAtAction(
            nameof(GetManager),
            new { restaurant_id, manager_id = manager.MasterId },
            response);
    }

    /// <summary>
    /// get restaurant's manager
    /// </summary>
    [HttpGet("{restaurant_id}/managers/{manager_id}")]
    public async Task<ActionResult<RestaurantManagerResponse>> GetManager(Guid restaurant_id, string manager_id)
    {
        var response = await restaurantService.GetManager(
            restaurant_id, manager_id,
            RestaurantManagerResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// update restaurant's manager
    /// </summary>
    [HttpPut("{restaurant_id}/managers/{manager_id}")]
    public async Task<ActionResult> UpdateManager(Guid restaurant_id, string manager_id, RestaurantManagerRequest body)
    {
        var manager = restaurantService.GetManagerStub(restaurant_id, manager_id);

        await restaurantService.SetManagerRoles(manager, body.roles);

        await restaurantService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// delete restaurant's manager
    /// </summary>
    [HttpDelete("{restaurant_id}/managers/{manager_id}")]
    public async Task<ActionResult> DeleteManager(Guid restaurant_id, string manager_id)
    {
        var affected = await restaurantService
            .QuerySingleManager(restaurant_id, manager_id)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }
}