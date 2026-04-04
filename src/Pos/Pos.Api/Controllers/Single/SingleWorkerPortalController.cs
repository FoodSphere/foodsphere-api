namespace FoodSphere.Pos.Api.Controller;

[Route("s/restaurants/{restaurant_id}")]
public class SingleWorkerPortalController(
    ILogger<WorkerController> logger,
    WorkerPortalService workerPortalService
) : PosControllerBase
{
    /// <summary>
    /// list worker's portals
    /// </summary>
    [HttpGet("worker-portals")]
    public async Task<ActionResult<ICollection<SingleWorkerPortalResponse>>> ListPortals(
        Guid restaurant_id)
    {
        return await workerPortalService.ListPortals(
            SingleWorkerPortalResponse.Projection, e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1);
    }

    /// <summary>
    /// get worker's portal
    /// </summary>
    [HttpGet("workers/{worker_id}/portal")]
    public async Task<ActionResult<SingleWorkerPortalResponse>> GetPortal(
        Guid restaurant_id, short worker_id)
    {
        var response = await workerPortalService.GetPortal(
            SingleWorkerPortalResponse.Projection,
            new(restaurant_id, 1, worker_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// upsert worker's portal
    /// </summary>
    [HttpPut("workers/{worker_id}/portal")]
    public async Task<ActionResult<SingleWorkerPortalResponse>> SetPortal(
        Guid restaurant_id, short worker_id,
        WorkerPortalRequest body)
    {
        var portalResult = await workerPortalService.SetPortal(
            SingleWorkerPortalResponse.Projection, new(
                new(restaurant_id, 1, worker_id),
                body.valid_duration));

        if (portalResult.IsFailed)
            return portalResult.Errors.ToActionResult();

        var (portalKey, response) = portalResult.Value;

        return CreatedAtAction(
            nameof(GetPortal),
            new { restaurant_id, worker_id, portal_id = portalKey.Id },
            response);
    }
}