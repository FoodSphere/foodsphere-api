namespace FoodSphere.Common.Service;

public class WorkerServiceBase(
    PersistenceService persistenceService,
    WorkerRepository workerRepository
) : ServiceBase
{
    public async Task<TDto[]> ListWorkers<TDto>(
        Expression<Func<WorkerUser, TDto>> projection,
        Expression<Func<WorkerUser, bool>> predicate,
        CancellationToken ct = default)
    {
        return await workerRepository.QueryWorkers()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<TDto?> GetWorker<TDto>(
        Expression<Func<WorkerUser, TDto>> projection, WorkerUserKey key,
        CancellationToken ct = default)
    {
        return await workerRepository.QuerySingleWorker(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject<(WorkerUserKey, TDto)>> CreateWorker<TDto>(
        Expression<Func<WorkerUser, TDto>> projection,
        WorkerCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await workerRepository.CreateWorker(
            branchKey: command.BranchKey,
            name: command.Name,
            phone: command.Phone,
            ct);

        if (!createResult.TryGetValue(out var worker))
            return createResult.Errors;

        var roleResult = await workerRepository.SetRoles(
            worker, command.RoleKeys, ct);

        if (roleResult.IsFailed)
            return roleResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetWorker(projection, worker, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created worker.");

        return (worker, response);
    }

    public async Task<ResultObject> DeleteWorker(
        WorkerUserKey key, CancellationToken ct = default)
    {
        var result = await workerRepository.DeleteWorker(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> UpdateWorker(
        WorkerUserKey key, WorkerUpdateCommand command,
        CancellationToken ct = default)
    {
        var worker = await workerRepository.GetWorker(key, ct);

        if (worker is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Worker not found.");

        worker.Name = command.Name;
        worker.Phone = command.Phone;

        var roleResult = await workerRepository.SetRoles(
            worker, command.RoleKeys, ct);

        if (roleResult.IsFailed)
            return roleResult.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> SetRoles(
        WorkerUserKey key, IEnumerable<RoleKey> roleKeys,
        CancellationToken ct = default)
    {
        var result = await workerRepository.SetRoles(
            key, roleKeys, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}