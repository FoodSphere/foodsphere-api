namespace FoodSphere.Common.Service;

public class BranchServiceBase(
    PersistenceService persistenceService,
    BranchRepository branchRepository
) : ServiceBase
{
    public async Task<TDto[]> ListBranches<TDto>(
        Expression<Func<Branch, TDto>> projection,
        Expression<Func<Branch, bool>> predicate,
        CancellationToken ct = default)
    {
        return await branchRepository.QueryBranches()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(BranchKey, TDto)>> CreateBranch<TDto>(
        Expression<Func<Branch, TDto>> projection,
        BranchCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await branchRepository.CreateBranch(
            restaurantKey: command.RestaurantKey,
            name: command.Name,
            displayName: command.DisplayName,
            address: command.Address,
            openingTime: command.OpeningTime,
            closingTime: command.ClosingTime,
            contact: command.Contact,
            ct);

        if (!createResult.TryGetValue(out var branch))
            return createResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetBranch(projection, branch, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created branch.");

        return (branch, response);
    }

    public async Task<TDto?> GetBranch<TDto>(
        Expression<Func<Branch, TDto>> projection, BranchKey key,
        CancellationToken ct = default)
    {
        return await branchRepository.QuerySingleBranch(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateBranch(
        BranchKey key, BranchUpdateCommand command,
        CancellationToken ct = default)
    {
        var branch = await branchRepository.GetBranch(key, ct);

        if (branch is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Branch not found.");

        branch.Name = command.Name;
        branch.DisplayName = command.DisplayName;
        branch.Address = command.Address;
        branch.OpeningTime = command.OpeningTime;
        branch.ClosingTime = command.ClosingTime;
        branch.Contact.Email = command.Contact?.Email;
        branch.Contact.Name = command.Contact?.Name;
        branch.Contact.Phone = command.Contact?.Phone;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> SoftDeleteBranch(
        BranchKey key, CancellationToken ct = default)
    {
        var deleteResult = await branchRepository.DeleteBranch(key, ct);

        if (deleteResult.IsFailed)
            return deleteResult.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}