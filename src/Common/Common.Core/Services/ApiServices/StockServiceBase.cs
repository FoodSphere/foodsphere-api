namespace FoodSphere.Common.Service;

public class StockServiceBase(
    PersistenceService persistenceService,
    StockTransactionRepository transactionRepository
) : ServiceBase
{
    public async Task<TDto[]> ListTransactions<TDto>(
        Expression<Func<StockTransaction, TDto>> projection,
        Expression<Func<StockTransaction, bool>> predicate,
        PaginationDto? pagination = null,
        CancellationToken ct = default)
    {
        var query = transactionRepository.QueryTransactions()
            .OrderByDescending(e => e.Id)
            .Where(predicate);

        if (pagination is not null)
            query = query
                .Skip(pagination.Offset * pagination.Limit)
                .Take(pagination.Limit);

        return await query
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<TDto[]> ListLatestTransactionOfIngredients<TDto>(
        Expression<Func<StockTransaction, TDto>> projection,
        Expression<Func<StockTransaction, bool>> predicate,
        CancellationToken ct = default)
    {
        var latestIds = transactionRepository.QueryTransactions()
            .Where(predicate)
            .GroupBy(e => new { e.RestaurantId, e.BranchId, e.IngredientId })
            .Select(g => g
                .OrderByDescending(e => e.Id)
                .Select(e => e.Id)
                .First());

        return await transactionRepository.QueryTransactions()
            .Where(predicate)
            .Where(e => latestIds.Contains(e.Id))
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<TDto?> GetTransaction<TDto>(
        Expression<Func<StockTransaction, TDto>> projection,
        StockTransactionKey key, CancellationToken ct = default)
    {
        return await transactionRepository.QuerySingleTransaction(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<TDto?> GetIngredientLatestTransaction<TDto>(
        Expression<Func<StockTransaction, TDto>> projection,
        BranchKey branchKey, IngredientKey ingredientKey,
        CancellationToken ct = default)
    {
        return await transactionRepository
            .QueryIngredientTransactions(branchKey, ingredientKey)
            .Select(projection)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<ResultObject<(StockTransactionKey, TDto)>> CreateTransaction<TDto>(
        Expression<Func<StockTransaction, TDto>> projection,
        StockCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await transactionRepository.CreateTransaction(
            branchKey: command.BranchKey,
            ingredientKey: command.IngredientKey,
            itemKey: null,
            amount: command.Amount,
            note: command.Note,
            ct: ct);

        if (!createResult.TryGetValue(out var transaction))
            return createResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetTransaction(projection, transaction, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created stock's transaction.");

        return (transaction, response);
    }

    public async Task<ResultObject<(StockTransactionKey, TDto)?>> RebalanceTransaction<TDto>(
        Expression<Func<StockTransaction, TDto>> projection,
        StockCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await transactionRepository.RebalanceTransaction(
            branchKey: command.BranchKey,
            ingredientKey: command.IngredientKey,
            targetAmount: command.Amount,
            note: command.Note,
            ct: ct);

        if (!createResult.TryGetValue(out var transaction))
            return createResult.Errors;

        if (transaction is null)
            return ResultObject.None();

        await persistenceService.Commit(ct);

        var response = await GetTransaction(projection, transaction, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created stock's transaction.");

        return (transaction, response);
    }
}