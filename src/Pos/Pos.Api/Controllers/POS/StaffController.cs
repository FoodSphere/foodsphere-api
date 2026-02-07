namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/branches/{branch_id}/staffs")]
public class StaffController(
    ILogger<StaffController> logger,
    BranchService branchService,
    StaffService staffService,
    StaffPortalService staffPortalService
) : PosControllerBase
{
    [HttpPost("{staff_id}/portal")]
    public async Task<ActionResult<StaffPortalResponse>> CreatePortal(Guid restaurant_id, short branch_id, short staff_id)
    {
        var staff = await staffService.GetStaff(restaurant_id, branch_id, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        var portal = await staffPortalService.CreateStaffPortal(staff);

        return StaffPortalResponse.FromModel(portal);
    }

    [HttpGet]
    public async Task<ActionResult<List<StaffResponse>>> ListStaffs(Guid restaurant_id, short branch_id)
    {
        var staffs = await staffService.ListStaffs(restaurant_id, branch_id);

        return staffs
            .Select(StaffResponse.FromModel)
            .ToList();
    }

    [HttpPost]
    public async Task<ActionResult<StaffResponse>> CreateStaff(Guid restaurant_id, short branch_id, StaffRequest body)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

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
            new { restaurant_id, branch_id, staff_id = staff.Id },
            StaffResponse.FromModel(staff)
        );
    }

    [HttpGet("{staff_id}")]
    public async Task<ActionResult<StaffResponse>> GetStaff(Guid restaurant_id, short branch_id, short staff_id)
    {
        var staff = await staffService.GetStaff(restaurant_id, branch_id, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        return StaffResponse.FromModel(staff);
    }

    [HttpDelete("{staff_id}")]
    public async Task<ActionResult> DeleteStaff(Guid restaurant_id, short branch_id, short staff_id)
    {
        var staff = await staffService.GetStaff(restaurant_id, branch_id, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        await staffService.DeleteStaff(staff);
        await staffService.SaveChanges();

        return NoContent();
    }
}