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
    /// create staff's portal
    /// </summary>
    [HttpPost("{staff_id}/portal")]
    public async Task<ActionResult<SingleStaffPortalResponse>> CreatePortal(Guid restaurant_id, short staff_id, StaffPortalRequest body)
    {
        var staff = await staffService.GetDefaultStaff(restaurant_id, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        var portal = await staffPortalService.CreateStaffPortal(staff, body.valid_duration);

        return SingleStaffPortalResponse.FromModel(portal);
    }

    /// <summary>
    /// list staffs
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<SingleStaffResponse[]>> ListStaffs(Guid restaurant_id)
    {
        var staffs = await staffService.ListDefaultStaffs(restaurant_id);

        return staffs
            .Select(SingleStaffResponse.FromModel)
            .ToArray();
    }

    /// <summary>
    /// create staff
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SingleStaffResponse>> CreateStaff(Guid restaurant_id, StaffRequest body)
    {
        var branch = await branchService.GetDefaultBranch(restaurant_id);

        if (branch is null)
        {
            return NotFound();
        }

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

        // get staff?

        return CreatedAtAction(
            nameof(GetStaff),
            new { restaurant_id, staff_id = staff.Id },
            SingleStaffResponse.FromModel(staff)
        );
    }

    /// <summary>
    /// get staff
    /// </summary>
    [HttpGet("{staff_id}")]
    public async Task<ActionResult<SingleStaffResponse>> GetStaff(Guid restaurant_id, short staff_id)
    {
        var staff = await staffService.GetDefaultStaff(restaurant_id, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        return SingleStaffResponse.FromModel(staff);
    }

    /// <summary>
    /// delete staff
    /// </summary>
    [HttpDelete("{staff_id}")]
    public async Task<ActionResult> DeleteStaff(Guid restaurant_id, short staff_id)
    {
        var staff = await staffService.GetDefaultStaff(restaurant_id, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        await staffService.DeleteStaff(staff);
        await staffService.SaveChanges();

        return NoContent();
    }
}