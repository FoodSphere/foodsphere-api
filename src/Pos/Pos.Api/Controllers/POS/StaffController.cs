namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/branches/{branch_id}/staffs")]
public class StaffController(
    ILogger<StaffController> logger,
    BranchService branchService,
    StaffService staffService,
    StaffPortalService staffPortalService
) : PosControllerBase
{
    /// <summary>
    /// list staffs
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<StaffResponse>>> ListStaffs(Guid restaurant_id, short branch_id)
    {
        var responses = await staffService.QueryStaffs()
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == branch_id)
            .Select(StaffResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create staff
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<StaffResponse>> CreateStaff(
        Guid restaurant_id, short branch_id, StaffRequest body)
    {
        var branch = branchService.GetBranchStub(restaurant_id, branch_id);

        var staff = await staffService.CreateStaff(
            branch: branch,
            name: body.name,
            phone: body.phone
        );

        await staffService.SetRoles(
            staff: staff,
            roleIds: body.roles
        );

        await staffService.SaveChanges();

        var response = await staffService.GetStaff(
            restaurant_id, branch_id, staff.Id,
            StaffResponse.Projection);

        return CreatedAtAction(
            nameof(GetStaff),
            new { restaurant_id, branch_id, staff_id = staff.Id },
            response);
    }

    /// <summary>
    /// get staff
    /// </summary>
    [HttpGet("{staff_id}")]
    public async Task<ActionResult<StaffResponse>> GetStaff(Guid restaurant_id, short branch_id, short staff_id)
    {
        var response = await staffService.GetStaff(
            restaurant_id, branch_id, staff_id,
            StaffResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// delete staff
    /// </summary>
    [HttpDelete("{staff_id}")]
    public async Task<ActionResult> DeleteStaff(Guid restaurant_id, short branch_id, short staff_id)
    {
        var affected = await staffService
            .QuerySingleStaff(restaurant_id, branch_id, staff_id)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// list staff's portals
    /// </summary>
    [HttpGet("{staff_id}/portals")]
    public async Task<ActionResult<ICollection<StaffPortalResponse>>> ListPortals(
        Guid restaurant_id, short branch_id, short staff_id)
    {
        var responses = await staffPortalService.QueryPortals()
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == branch_id &&
                e.StaffId == staff_id)
            .Select(StaffPortalResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create staff's portal
    /// </summary>
    [HttpPost("{staff_id}/portal")]
    public async Task<ActionResult<StaffPortalResponse>> CreatePortal(
        Guid restaurant_id, short branch_id, short staff_id,
        StaffPortalRequest body)
    {
        var staff = staffService.GetStaffStub(restaurant_id, branch_id, staff_id);

        var portal = await staffPortalService.CreatePortal(staff, body.valid_duration);

        await staffPortalService.SaveChanges();

        var response = StaffPortalResponse.Project(portal);

        return CreatedAtAction(
            nameof(GetPortal),
            new { restaurant_id, branch_id, staff_id, portal_id = portal.Id },
            response);
    }

    /// <summary>
    /// get staff's portal
    /// </summary>
    [HttpGet("{staff_id}/portals/{portal_id}")]
    public async Task<ActionResult<StaffPortalResponse>> GetPortal(
        Guid restaurant_id, short branch_id, short staff_id, Guid portal_id)
    {
        var response = await staffPortalService.QuerySinglePortal(portal_id)
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == branch_id &&
                e.StaffId == staff_id)
            .Select(StaffPortalResponse.Projection)
            .SingleOrDefaultAsync();

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }
}