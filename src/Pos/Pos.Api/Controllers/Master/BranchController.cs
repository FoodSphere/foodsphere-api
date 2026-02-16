namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/branches")]
public class BranchController(
    ILogger<BranchController> logger,
    BranchService branchService
) : MasterControllerBase
{
    /// <summary>
    /// list branches
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<BranchResponse>>> ListBranches(Guid restaurant_id)
    {
        var responses = await branchService
            .QueryBranches()
            .Where(e =>
                e.RestaurantId == restaurant_id)
            .Select(BranchResponse.Projection)
            .ToArrayAsync();

        return responses;
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

        var response = await branchService.GetBranch(
            restaurant_id, branch.Id,
            BranchResponse.Projection);

        return CreatedAtAction(
            nameof(InfoController.GetBranch),
            GetControllerName(nameof(InfoController)),
            new { restaurant_id, branch_id = branch.Id },
            response);
    }

    /// <summary>
    /// update branch
    /// </summary>
    [HttpPut("{branch_id}")]
    public async Task<ActionResult> UpdateBranch(Guid restaurant_id, short branch_id, BranchRequest body)
    {
        var branch = branchService.GetBranchStub(restaurant_id, branch_id);

        // if (branch.Restaurant.OwnerId != MasterId)
        // {
        //     return Forbid();
        // }

        branch.Name = body.name;
        branch.DisplayName = body.display_name;
        branch.Address = body.address;
        branch.OpeningTime = body.opening_time;
        branch.ClosingTime = body.closing_time;

        if (body.contact is not null)
        {
            await branchService.SetContact(branch, body.contact);
        }

        var affected = await branchService.SaveChanges();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// delete branch
    /// </summary>
    [HttpDelete("{branch_id}")]
    public async Task<ActionResult> DeleteBranch(Guid restaurant_id, short branch_id)
    {
        var affected = await branchService
            .QuerySingleBranch(restaurant_id, branch_id)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// list branch's managers
    /// </summary>
    [HttpGet("{branch_id}/managers")]
    public async Task<ActionResult<ICollection<BranchManagerResponse>>> ListManagers(Guid restaurant_id, short branch_id)
    {
        var response = await branchService
            .QueryManagers()
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == branch_id)
            .Select(BranchManagerResponse.Projection)
            .ToArrayAsync();

        return response;
    }

    /// <summary>
    /// create branch's manager
    /// </summary>
    [HttpPost("{branch_id}/managers")]
    public async Task<ActionResult<BranchManagerResponse>> CreateManager(
        Guid restaurant_id,
        short branch_id,
        ManagerRequest body)
    {
        var manager = await branchService.CreateManager(
            restaurantId: restaurant_id,
            branchId: branch_id,
            masterId: body.master_id
        );

        await branchService.SetManagerRoles(manager, body.roles);

        await branchService.SaveChanges();

        var response = await branchService.GetManager(
            restaurant_id, branch_id, body.master_id,
            BranchManagerResponse.Projection);

        return CreatedAtAction(
            nameof(GetManager),
            new { restaurant_id, branch_id, master_id = body.master_id },
            response);
    }

    /// <summary>
    /// get branch's manager
    /// </summary>
    [HttpGet("{branch_id}/managers/{master_id}")]
    public async Task<ActionResult<BranchManagerResponse>> GetManager(
        Guid restaurant_id,
        short branch_id,
        string master_id)
    {
        var response = await branchService.GetManager(
            restaurant_id, branch_id, master_id,
            BranchManagerResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// update branch's manager
    /// </summary>
    [HttpPut("{branch_id}/managers/{manager_id}")]
    public async Task<ActionResult> UpdateManager(
        Guid restaurant_id,
        short branch_id,
        string manager_id,
        RestaurantManagerRequest body)
    {
        var manager = branchService.GetManagerStub(restaurant_id, branch_id, manager_id);

        await branchService.SetManagerRoles(manager, body.roles);

        await branchService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// delete branch's manager
    /// </summary>
    [HttpDelete("{branch_id}/managers/{manager_id}")]
    public async Task<ActionResult> DeleteManager(
        Guid restaurant_id,
        short branch_id,
        string manager_id)
    {
        var affected = await branchService
            .QuerySingleManager(restaurant_id, branch_id, manager_id)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }
}