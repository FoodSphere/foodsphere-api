namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants")]
public class RestaurantController(
    ILogger<RestaurantController> logger,
    RestaurantService restaurantService,
    BranchService branchService
) : MasterControllerBase
{
    [HttpPost]
    public async Task<ActionResult<QuickRestaurantResponse>> CreateRestaurant(QuickRestaurantRequest body)
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

        var branch = await branchService.CreateBranch(
            restaurantId: restaurant.Id,
            name: "main",
            address: body.address,
            openingTime: body.opening_time,
            closingTime: body.closing_time
        );

        await branchService.SaveAsync();

        return CreatedAtAction(
            nameof(StaffAccessController.GetRestaurant),
            GetControllerName(nameof(StaffAccessController)),
            new { restaurant_id = restaurant.Id },
            QuickRestaurantResponse.FromModel(restaurant, branch)
        );
    }

    [HttpGet]
    public async Task<ActionResult<List<RestaurantResponse>>> ListMyRestaurants()
    {
        var restaurant = await restaurantService.ListRestaurants(MasterId);

        return restaurant
            .Select(RestaurantResponse.FromModel)
            .ToList();
    }

    [HttpPost("{restaurant_id}/contact")]
    public async Task<ActionResult<RestaurantResponse>> SetContact(Guid restaurant_id, ContactDto body)
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
        await restaurantService.SaveAsync();

        return NoContent();
    }

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
        await restaurantService.SaveAsync();

        return NoContent();
    }
}