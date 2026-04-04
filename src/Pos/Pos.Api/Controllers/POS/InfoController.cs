namespace FoodSphere.Pos.Api.Controller;

public class InfoController(
    ILogger<InfoController> logger,
    PermissionServiceBase permissionService,
    RestaurantServiceBase restaurantService,
    BranchServiceBase branchService
) : PosControllerBase
{
    /// <summary>
    /// list permissions
    /// </summary>
    [HttpGet("permissions")]
    public async Task<ActionResult<ICollection<PermissionResponse>>> ListPermissions()
    {
        return await permissionService.ListPermissions(
            PermissionResponse.Projection, e => true);
    }

    /// <summary>
    /// get restaurant
    /// </summary>
    [HttpGet("restaurants/{restaurant_id}")]
    public async Task<ActionResult<RestaurantResponse>> GetRestaurant(
        Guid restaurant_id)
    {
        var response = await restaurantService.GetRestaurant(
            RestaurantResponse.Projection,
            new(restaurant_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// get branch
    /// </summary>
    [HttpGet("restaurants/{restaurant_id}/branches/{branch_id}")]
    public async Task<ActionResult<BranchResponse>> GetBranch(
        Guid restaurant_id, short branch_id)
    {
        var response = await branchService.GetBranch(
            BranchResponse.Projection,
            new(restaurant_id, branch_id));

        if (response is null)
            return NotFound();

        return response;
    }
}