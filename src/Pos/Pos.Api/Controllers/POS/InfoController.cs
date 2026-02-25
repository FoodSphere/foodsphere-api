namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}")]
public class InfoController(
    ILogger<InfoController> logger,
    RestaurantService restaurantService,
    BranchService branchService
) : PosControllerBase
{
    /// <summary>
    /// get restaurant
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<RestaurantResponse>> GetRestaurant(Guid restaurant_id)
    {
        var restaurant = await restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        return RestaurantResponse.FromModel(restaurant);
    }

    /// <summary>
    /// get branch
    /// </summary>
    [HttpGet("branches/{branch_id}")]
    public async Task<ActionResult<BranchResponse>> GetBranch(Guid restaurant_id, short branch_id)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        return BranchResponse.FromModel(branch);
    }
}