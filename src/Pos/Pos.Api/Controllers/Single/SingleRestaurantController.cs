namespace FoodSphere.Pos.Api.Controller;

[Route("s/restaurants")]
public class SingleRestaurantController(
    ILogger<SingleRestaurantController> logger,
    RestaurantService restaurantService,
    BranchService branchService
) : MasterControllerBase
{
    /// <summary>
    /// create restaurant
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SingleRestaurantResponse>> CreateRestaurant(SingleRestaurantRequest body)
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
            name: "default",
            address: body.address,
            openingTime: body.opening_time,
            closingTime: body.closing_time
        );

        await branchService.SaveChanges();

        var response = await branchService.GetBranch(
            restaurant.Id, 1,
            SingleRestaurantResponse.Projection);

        return CreatedAtAction(
            nameof(SingleInfoController.GetRestaurant),
            GetControllerName(nameof(SingleInfoController)),
            new { restaurant_id = restaurant.Id },
            response);
    }

    /// <summary>
    /// list owned restaurants
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<SingleRestaurantResponse>>> ListOwnedRestaurants()
    {
        var responses = await branchService.QueryBranches()
            .Where(e =>
                e.Restaurant.OwnerId == MasterId &&
                e.Id == 1)
            .Select(SingleRestaurantResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// update restaurant
    /// </summary>
    [HttpPut("{restaurant_id}")]
    public async Task<ActionResult<SingleRestaurantResponse>> UpdateRestaurant(Guid restaurant_id, SingleRestaurantRequest body)
    {
        var branch = branchService.GetBranchStub(restaurant_id, 1);

        branch.Restaurant.Name = body.name;
        branch.Restaurant.DisplayName = body.display_name;
        branch.Address = body.address;
        branch.OpeningTime = body.opening_time;
        branch.ClosingTime = body.closing_time;

        if (body.contact is not null)
        {
            var restaurant = restaurantService.GetRestaurantStub(restaurant_id);
            await restaurantService.SetContact(restaurant, body.contact);
        }

        var affected = await branchService.SaveChanges();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }
}