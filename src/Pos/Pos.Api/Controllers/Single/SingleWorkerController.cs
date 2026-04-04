namespace FoodSphere.Pos.Api.Controller;

[Route("s/restaurants/{restaurant_id}/workers")]
public class SingleWorkerController(
    ILogger<SingleWorkerController> logger,
    WorkerServiceBase workerService,
    WorkerPortalService workerPortalService
) : PosControllerBase
{
    /// <summary>
    /// list workers
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<SingleWorkerResponse>>> ListWorkers(
        Guid restaurant_id,
        [FromQuery] bool? is_deleted = false)
    {
        Expression<Func<WorkerUser, bool>> predicate = e =>
            e.RestaurantId == restaurant_id &&
            e.BranchId == 1;

        if (is_deleted is not null)
            predicate = predicate.And(e => e.DeleteTime != null == is_deleted.Value);

        return await workerService.ListWorkers(
            SingleWorkerResponse.Projection, predicate);
    }

    /// <summary>
    /// create worker
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SingleWorkerResponse>> CreateWorker(
        Guid restaurant_id, WorkerRequest body)
    {
        var roleKeys = body.roles.Select(role_id =>
            new RoleKey(restaurant_id, role_id));

        var workerResult = await workerService.CreateWorker(
            WorkerResponse.Projection, new(
                new(restaurant_id, 1),
                body.name,
                roleKeys,
                body.phone));

        if (workerResult.IsFailed)
            return workerResult.Errors.ToActionResult();

        var (workerKey, response) = workerResult.Value;

        return CreatedAtAction(
            nameof(GetWorker),
            new { restaurant_id, worker_id = workerKey.Id },
            response);
    }

    /// <summary>
    /// get worker
    /// </summary>
    [HttpGet("{worker_id}")]
    public async Task<ActionResult<SingleWorkerResponse>> GetWorker(
        Guid restaurant_id, short worker_id)
    {
        var response = await workerService.GetWorker(
            SingleWorkerResponse.Projection,
            new(restaurant_id, 1, worker_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// update worker
    /// </summary>
    [HttpPut("{worker_id}")]
    public async Task<ActionResult> UpdateWorker(
        Guid restaurant_id, short worker_id,
        WorkerRequest body)
    {
        var roleKeys = body.roles.Select(role_id =>
            new RoleKey(restaurant_id, role_id));

        var result = await workerService.UpdateWorker(
            new(restaurant_id, 1, worker_id), new(
                body.name,
                roleKeys,
                body.phone));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// delete worker
    /// </summary>
    [HttpDelete("{worker_id}")]
    public async Task<ActionResult> DeleteWorker(
        Guid restaurant_id, short worker_id)
    {
        var result = await workerService.DeleteWorker(
            new(restaurant_id, 1, worker_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}