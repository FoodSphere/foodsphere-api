namespace FoodSphere.Pos.Api.Controller;

[Route("s/restaurants/{restaurant_id}")]
public class SingleInfoController(
    ILogger<SingleInfoController> logger,
    // RestaurantService restaurantService,
    BranchService branchService
) : PosControllerBase
{
    /// <summary>
    /// get restaurant
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<SingleRestaurantResponse>> GetRestaurant(Guid restaurant_id)
    {
        var response = await branchService.GetBranch(
            restaurant_id, 1,
            SingleRestaurantResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }
}