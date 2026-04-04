namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/branches/{branch_id}")]
public class WorkerPortalController(
    ILogger<WorkerPortalController> logger,
    WorkerPortalService workerPortalService
) : PosControllerBase
{
    /// <summary>
    /// list worker's portals
    /// </summary>
    [HttpGet("worker-portals")]
    public async Task<ActionResult<ICollection<WorkerPortalResponse>>> ListPortals(
        Guid restaurant_id, short branch_id)
    {
        return await workerPortalService.ListPortals(
            WorkerPortalResponse.Projection, e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == branch_id);
    }

    /// <summary>
    /// get worker's portal
    /// </summary>
    [HttpGet("workers/{worker_id}/portal")]
    public async Task<ActionResult<WorkerPortalResponse>> GetPortal(
        Guid restaurant_id, short branch_id, short worker_id)
    {
        var response = await workerPortalService.GetPortal(
            WorkerPortalResponse.Projection,
            new(restaurant_id, branch_id, worker_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// upsert worker's portal
    /// </summary>
    [HttpPut("workers/{worker_id}/portal")]
    public async Task<ActionResult<WorkerPortalResponse>> SetPortal(
        Guid restaurant_id, short branch_id, short worker_id,
        WorkerPortalRequest body)
    {
        var portalResult = await workerPortalService.SetPortal(
            WorkerPortalResponse.Projection, new(
                new(restaurant_id, branch_id, worker_id),
                body.valid_duration));

        if (portalResult.IsFailed)
            return portalResult.Errors.ToActionResult();

        var (portalKey, response) = portalResult.Value;

        return CreatedAtAction(
            nameof(GetPortal),
            new { restaurant_id, branch_id, worker_id, portal_id = portalKey.Id },
            response);
    }
}