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
    public async Task<ActionResult<ICollection<PermissionResponse>>> ListPermissions()
    {
        var permissions = await permissionService.ListPermissions();

        return permissions
            .Select(PermissionResponse.Project)
            .ToArray();
    }

    /// <summary>
    /// get restaurant
    /// </summary>
    [HttpGet("restaurants/{restaurant_id}")]
    public async Task<ActionResult<RestaurantResponse>> GetRestaurant(Guid restaurant_id)
    {
        var response = await restaurantService.GetRestaurant(restaurant_id, RestaurantResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// get branch
    /// </summary>
    [HttpGet("restaurants/{restaurant_id}/branches/{branch_id}")]
    public async Task<ActionResult<BranchResponse>> GetBranch(Guid restaurant_id, short branch_id)
    {
        var response = await branchService.GetBranch(restaurant_id, branch_id, BranchResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }
}