namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants")]
public class RestaurantStaffController(
    ILogger<RestaurantStaffController> logger,
    RestaurantStaffServiceBase staffService
) : MasterControllerBase
{
    /// <summary>
    /// list restaurant's staffs
    /// </summary>
    [HttpGet("{restaurant_id}/staffs")]
    public async Task<ActionResult<ICollection<RestaurantStaffResponse>>> ListStaffs(
        Guid restaurant_id,
        [FromQuery] bool? is_deleted = false)
    {
        Expression<Func<RestaurantStaff, bool>> predicate = e =>
            e.RestaurantId == restaurant_id;

        if (is_deleted is not null)
            predicate = predicate.And(e => e.DeleteTime != null == is_deleted.Value);

        return await staffService.ListStaffs(
            RestaurantStaffResponse.Projection, predicate);
    }

    /// <summary>
    /// create restaurant's staff
    /// </summary>
    [HttpPost("{restaurant_id}/staffs")]
    public async Task<ActionResult<RestaurantStaffResponse>> CreateStaff(
        Guid restaurant_id, StaffRequest body)
    {
        var roleKeys = body.roles.Select(role_id =>
            new RoleKey(restaurant_id, role_id));

        var createResult = await staffService.CreateStaff(
            RestaurantStaffResponse.Projection, new(
                new(restaurant_id),
                new(body.master_id),
                body.display_name,
                roleKeys));

        if (createResult.IsFailed)
            return createResult.Errors.ToActionResult();

        var (_, response) = createResult.Value;

        return CreatedAtAction(
            nameof(GetStaff),
            new { restaurant_id, master_id = body.master_id },
            response);
    }

    /// <summary>
    /// get restaurant's staff
    /// </summary>
    [HttpGet("{restaurant_id}/staffs/{master_id}")]
    public async Task<ActionResult<RestaurantStaffResponse>> GetStaff(
        Guid restaurant_id, string master_id)
    {
        var response = await staffService.GetStaff(
            RestaurantStaffResponse.Projection,
            new(restaurant_id, master_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// set restaurant staff's role
    /// </summary>
    [HttpPut("{restaurant_id}/staffs/{master_id}/roles")]
    public async Task<ActionResult> SetStaffRoles(
        Guid restaurant_id, string master_id,
        UpdateStaffRequest body)
    {
        var roles = body.roles.Select(id =>
            new RoleKey(restaurant_id, id));

        var result = await staffService.SetStaffRoles(
            new(restaurant_id, master_id),
            roles);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// delete restaurant's staff
    /// </summary>
    [HttpDelete("{restaurant_id}/staffs/{master_id}")]
    public async Task<ActionResult> DeleteStaff(
        Guid restaurant_id, string master_id)
    {
        var result = await staffService.DeleteStaff(
            new(restaurant_id, master_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}