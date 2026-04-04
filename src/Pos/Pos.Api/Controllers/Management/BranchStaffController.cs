namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/branches")]
public class BranchStaffController(
    ILogger<BranchStaffController> logger,
    BranchStaffServiceBase staffService
) : MasterControllerBase
{
    /// <summary>
    /// list branch's staffs
    /// </summary>
    [HttpGet("{branch_id}/staffs")]
    public async Task<ActionResult<ICollection<BranchStaffResponse>>> ListStaffs(
        Guid restaurant_id, short branch_id,
        [FromQuery] bool? is_deleted = false)
    {
        Expression<Func<BranchStaff, bool>> predicate = e =>
            e.RestaurantId == restaurant_id &&
            e.BranchId == branch_id;

        if (is_deleted is not null)
            predicate = predicate.And(e => e.DeleteTime != null == is_deleted.Value);

        return await staffService.ListStaffs(
            BranchStaffResponse.Projection, predicate);
    }

    /// <summary>
    /// create branch's staff
    /// </summary>
    [HttpPost("{branch_id}/staffs")]
    public async Task<ActionResult<BranchStaffResponse>> CreateStaff(
        Guid restaurant_id, short branch_id, StaffRequest body)
    {
        var roleKeys = body.roles.Select(role_id =>
            new RoleKey(restaurant_id, role_id));

        var createResult = await staffService.CreateStaff(
            BranchStaffResponse.Projection, new(
                new(restaurant_id, branch_id),
                new(body.master_id),
                body.display_name,
                roleKeys));

        if (createResult.IsFailed)
            return createResult.Errors.ToActionResult();

        var (_, response) = createResult.Value;

        return CreatedAtAction(
            nameof(GetStaff),
            new { restaurant_id, branch_id, master_id = body.master_id },
            response);
    }

    /// <summary>
    /// get branch's staff
    /// </summary>
    [HttpGet("{branch_id}/staffs/{master_id}")]
    public async Task<ActionResult<BranchStaffResponse>> GetStaff(
        Guid restaurant_id,
        short branch_id,
        string master_id)
    {
        var response = await staffService.GetStaff(
            BranchStaffResponse.Projection,
            new(restaurant_id, branch_id, master_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// set branch staff's role
    /// </summary>
    [HttpPut("{branch_id}/staffs/{master_id}")]
    public async Task<ActionResult> SetStaffRoles(
        Guid restaurant_id, short branch_id, string master_id,
        UpdateStaffRequest body)
    {
        var roles = body.roles.Select(role_id =>
            new RoleKey(restaurant_id, role_id));

        var result = await staffService.SetStaffRoles(
            new(restaurant_id, branch_id, master_id),
            roles);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// delete branch's staff
    /// </summary>
    [HttpDelete("{branch_id}/staffs/{master_id}")]
    public async Task<ActionResult> DeleteStaff(
        Guid restaurant_id, short branch_id, string master_id)
    {
        var result = await staffService.DeleteStaff(
            new(restaurant_id, branch_id, master_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}