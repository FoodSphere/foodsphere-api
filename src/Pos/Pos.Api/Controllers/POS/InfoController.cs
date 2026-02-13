namespace FoodSphere.Pos.Api.Controller;

public class InfoController(
    ILogger<InfoController> logger,
    PermissionService permissionService,
    RestaurantService restaurantService,
    BranchService branchService
) : PosControllerBase
{
    /// <summary>
    /// list permissions
    /// </summary>
    [HttpGet("permissions")]
    public async Task<ActionResult<List<PermissionResponse>>> ListPermissions()
    {
        var permissions = await permissionService.ListPermissions();

        return permissions
            .Select(PermissionResponse.FromModel)
            .ToList();
    }

    /// <summary>
    /// get restaurant
    /// </summary>
    [HttpGet("restaurants/{restaurant_id}")]
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
    [HttpGet("restaurants/{restaurant_id}/branches/{branch_id}")]
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