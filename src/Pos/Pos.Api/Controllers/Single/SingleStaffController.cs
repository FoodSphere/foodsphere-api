namespace FoodSphere.Pos.Api.Controller;

[Route("s/restaurants/{restaurant_id}/staffs")]
public class SingleStaffController(
    ILogger<SingleStaffController> logger,
    BranchService branchService,
    StaffService staffService,
    StaffPortalService staffPortalService
) : PosControllerBase
{
    /// <summary>
    /// list staffs
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<SingleStaffResponse>>> ListStaffs(Guid restaurant_id)
    {
        var responses = await staffService.QueryStaffs()
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1)
            .Select(SingleStaffResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create staff
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SingleStaffResponse>> CreateStaff(Guid restaurant_id, StaffRequest body)
    {
        var branch = branchService.GetBranchStub(restaurant_id, 1);

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
            restaurant_id, 1, staff.Id,
            SingleStaffResponse.Projection);

        return CreatedAtAction(
            nameof(GetStaff),
            new { restaurant_id, staff_id = staff.Id },
            response);
    }

    /// <summary>
    /// get staff
    /// </summary>
    [HttpGet("{staff_id}")]
    public async Task<ActionResult<SingleStaffResponse>> GetStaff(Guid restaurant_id, short staff_id)
    {
        var response = await staffService.GetStaff(
            restaurant_id, 1, staff_id,
            SingleStaffResponse.Projection);

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
    public async Task<ActionResult> DeleteStaff(Guid restaurant_id, short staff_id)
    {
        var staff = await staffService.GetStaff(restaurant_id, 1, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        await staffService.DeleteStaff(staff);
        await staffService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// list staff's portals
    /// </summary>
    [HttpGet("{staff_id}/portals")]
    public async Task<ActionResult<ICollection<SingleStaffPortalResponse>>> ListPortals(Guid restaurant_id, short staff_id)
    {
        var responses = await staffPortalService.QueryPortals()
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1 &&
                e.StaffId == staff_id)
            .Select(SingleStaffPortalResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create staff's portal
    /// </summary>
    [HttpPost("{staff_id}/portal")]
    public async Task<ActionResult<SingleStaffPortalResponse>> CreatePortal(
        Guid restaurant_id, short staff_id, StaffPortalRequest body)
    {
        var staff = staffService.GetStaffStub(restaurant_id, 1, staff_id);

        var portal = await staffPortalService.CreatePortal(staff, body.valid_duration);

        await staffPortalService.SaveChanges();

        var response = StaffPortalResponse.Project(portal);

        return CreatedAtAction(
            nameof(GetPortal),
            new { restaurant_id, staff_id, portal_id = portal.Id },
            response);
    }

    /// <summary>
    /// get staff's portal
    /// </summary>
    [HttpGet("{staff_id}/portals/{portal_id}")]
    public async Task<ActionResult<SingleStaffPortalResponse>> GetPortal(
        Guid restaurant_id, short staff_id, Guid portal_id)
    {
        var response = await staffPortalService.QuerySinglePortal(portal_id)
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1 &&
                e.StaffId == staff_id)
            .Select(SingleStaffPortalResponse.Projection)
            .SingleOrDefaultAsync();

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }
}