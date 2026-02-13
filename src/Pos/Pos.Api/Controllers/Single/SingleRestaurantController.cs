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

        return CreatedAtAction(
            nameof(SingleInfoController.GetRestaurant),
            GetControllerName(nameof(SingleInfoController)),
            new { restaurant_id = restaurant.Id },
            SingleRestaurantResponse.FromModel(branch)
        );
    }

    /// <summary>
    /// list owned restaurants
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<SingleRestaurantResponse[]>> ListMyRestaurants()
    {
        var branches = await branchService.ListDefaultBranches(MasterId);

        return branches
            .Select(SingleRestaurantResponse.FromModel)
            .ToArray();
    }

    /// <summary>
    /// update restaurant
    /// </summary>
    [HttpPut("{restaurant_id}")]
    public async Task<ActionResult<SingleRestaurantResponse>> UpdateRestaurant(Guid restaurant_id, SingleRestaurantRequest body)
    {
        var branch = await branchService.GetDefaultBranch(restaurant_id);

        if (branch is null)
        {
            return NotFound();
        }

        if (body.contact is not null)
        {
            await restaurantService.SetContact(branch.Restaurant, body.contact);
        }

        branch.Restaurant.Name = body.name;
        branch.Restaurant.DisplayName = body.display_name;
        branch.Address = body.address;
        branch.OpeningTime = body.opening_time;
        branch.ClosingTime = body.closing_time;

        await restaurantService.SaveChanges();

        return SingleRestaurantResponse.FromModel(branch);
    }
}