namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/branches/{branch_id}/workers")]
public class WorkerController(
    ILogger<WorkerController> logger,
    WorkerServiceBase workerService
) : PosControllerBase
{
    /// <summary>
    /// list workers
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<WorkerResponse>>> ListWorkers(
        Guid restaurant_id, short branch_id,
        [FromQuery] bool? is_deleted = false)
    {
        Expression<Func<WorkerUser, bool>> predicate = e =>
            e.RestaurantId == restaurant_id &&
            e.BranchId == branch_id;

        if (is_deleted is not null)
            predicate = predicate.And(e => e.DeleteTime != null == is_deleted.Value);

        return await workerService.ListWorkers(
            WorkerResponse.Projection, predicate);
    }

    /// <summary>
    /// create worker
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WorkerResponse>> CreateWorker(
        Guid restaurant_id, short branch_id, WorkerRequest body)
    {
        var roleKeys = body.roles.Select(role_id =>
            new RoleKey(restaurant_id, role_id));

        var workerResult = await workerService.CreateWorker(
            WorkerResponse.Projection, new(
                new(restaurant_id, branch_id),
                body.name,
                roleKeys,
                body.phone));

        if (workerResult.IsFailed)
            return workerResult.Errors.ToActionResult();

        var (workerKey, response) = workerResult.Value;

        return CreatedAtAction(
            nameof(GetWorker),
            new { restaurant_id, branch_id, worker_id = workerKey.Id },
            response);
    }

    /// <summary>
    /// get worker
    /// </summary>
    [HttpGet("{worker_id}")]
    public async Task<ActionResult<WorkerResponse>> GetWorker(
        Guid restaurant_id, short branch_id, short worker_id)
    {
        var response = await workerService.GetWorker(
            WorkerResponse.Projection,
            new(restaurant_id, branch_id, worker_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// update worker
    /// </summary>
    [HttpPut("{worker_id}")]
    public async Task<ActionResult<WorkerResponse>> UpdateWorker(
        Guid restaurant_id, short branch_id, short worker_id,
        WorkerRequest body)
    {
        var roleKeys = body.roles.Select(role_id =>
            new RoleKey(restaurant_id, role_id));

        var result = await workerService.UpdateWorker(
            new(restaurant_id, branch_id, worker_id), new(
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
        Guid restaurant_id, short branch_id, short worker_id)
    {
        var result = await workerService.DeleteWorker(
            new(restaurant_id, branch_id, worker_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}