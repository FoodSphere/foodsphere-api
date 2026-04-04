namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/branches")]
public class BranchController(
    ILogger<BranchController> logger,
    BranchServiceBase branchService
) : MasterControllerBase
{
    /// <summary>
    /// list branches
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<BranchResponse>>> ListBranches(
        Guid restaurant_id,
        [FromQuery] bool? is_deleted = false)
    {
        Expression<Func<Branch, bool>> predicate = e =>
            e.RestaurantId == restaurant_id;

        if (is_deleted is not null)
            predicate = predicate.And(e => e.DeleteTime != null == is_deleted.Value);

        return await branchService.ListBranches(
            BranchResponse.Projection, predicate);
    }

    /// <summary>
    /// create branch
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BranchResponse>> CreateBranch(
        Guid restaurant_id, BranchRequest body)
    {
        var branchResult = await branchService.CreateBranch(
            BranchResponse.Projection, new(
                new(restaurant_id),
                body.name,
                body.display_name,
                body.address,
                body.opening_time,
                body.closing_time,
                body.contact));

        if (branchResult.IsFailed)
            return branchResult.Errors.ToActionResult();

        var (branchKey, response) = branchResult.Value;

        return CreatedAtAction(
            nameof(InfoController.GetBranch),
            GetControllerName(nameof(InfoController)),
            new { restaurant_id, branch_id = branchKey.Id },
            response);
    }

    /// <summary>
    /// update branch
    /// </summary>
    [HttpPut("{branch_id}")]
    public async Task<ActionResult> UpdateBranch(
        Guid restaurant_id, short branch_id, BranchRequest body)
    {
        var branchResult = await branchService.UpdateBranch(
             new(restaurant_id, branch_id), new(
                 body.name,
                 body.display_name,
                 body.address,
                 body.opening_time,
                 body.closing_time,
                 body.contact));

        if (branchResult.IsFailed)
            return branchResult.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// delete branch
    /// </summary>
    [HttpDelete("{branch_id}")]
    public async Task<ActionResult> DeleteBranch(
        Guid restaurant_id, short branch_id)
    {
        var result = await branchService.SoftDeleteBranch(
            new(restaurant_id, branch_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}