namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/branches")]
public class MasterBranchController(
    ILogger<MasterBranchController> logger,
    BranchService branchService
) : MasterControllerBase
{
    /// <summary>
    /// list branches
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<BranchResponse>>> ListBranches(Guid restaurant_id)
    {
        var branches = await branchService.ListBranches(restaurant_id);

        return branches
            .Select(BranchResponse.FromModel)
            .ToList();
    }

    /// <summary>
    /// create branch
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BranchResponse>> CreateBranch(Guid restaurant_id, BranchRequest body)
    {
        var branch = await branchService.CreateBranch(
            restaurantId: restaurant_id,
            name: body.name,
            displayName: body.display_name,
            address: body.address,
            openingTime: body.opening_time,
            closingTime: body.closing_time
        );

        if (body.contact is not null)
        {
            await branchService.SetContact(branch, body.contact);
        }

        await branchService.SaveChanges();

        return CreatedAtAction(
            nameof(InfoController.GetBranch),
            GetControllerName(nameof(InfoController)),
            new { restaurant_id, branch_id = branch.Id },
            BranchResponse.FromModel(branch)
        );
    }

    /// <summary>
    /// list branch managers
    /// </summary>
    [HttpGet("managers")]
    public async Task<ActionResult<List<BranchManagerResponse>>> ListManagers(Guid restaurant_id, short branch_id)
    {
        var managers = await branchService.ListManagers(restaurant_id, branch_id);

        return managers
            .Select(BranchManagerResponse.FromModel)
            .ToList();
    }

    /// <summary>
    /// not done
    /// </summary>
    [HttpPost("managers")]
    public async Task<ActionResult<BranchManagerResponse>> CreateManager(Guid restaurant_id, short branch_id, BranchManagerRequest body)
    {
        var manager = await branchService.CreateManager(restaurant_id, branch_id, body.master_id);

        await branchService.SaveChanges();

        return BranchManagerResponse.FromModel(manager);
    }

    /// <summary>
    /// delete branch
    /// </summary>
    [HttpDelete("{branch_id}")]
    public async Task<ActionResult> DeleteBranch(Guid restaurant_id, short branch_id)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        await branchService.DeleteBranch(branch);
        await branchService.SaveChanges();

        return NoContent();
    }
}