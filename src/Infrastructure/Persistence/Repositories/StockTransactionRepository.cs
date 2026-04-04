namespace FoodSphere.Infrastructure.Repository;

public class StockTransactionRepository(
    FoodSphereDbContext context,
    IPublishEndpoint publishEndpoint
    // OrderQueryService orderQueryService
) : RepositoryBase(context)
{
    async Task<decimal?> GetLatestBalance(
        BranchKey branchKey, IngredientKey ingredientKey,
        CancellationToken ct = default)
    {
        return await QueryIngredientTransactions(branchKey, ingredientKey)
            .Select(e => (decimal?)e.BalanceAfter)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<ResultObject<StockTransaction>> CreateTransaction(
        BranchKey branchKey,
        IngredientKey ingredientKey,
        OrderItemKey? itemKey,
        decimal amount,
        string? note,
        CancellationToken ct = default)
    {
        var latestBalance = await GetLatestBalance(branchKey, ingredientKey, ct);

        var transaction = new StockTransaction
        {
            RestaurantId = branchKey.RestaurantId,
            BranchId = branchKey.Id,
            Id = Guid.CreateVersion7(),
            IngredientId = ingredientKey.Id,
            BillId = itemKey?.BillId,
            OrderId = itemKey?.OrderId,
            OrderItemId = itemKey?.Id,
            Amount = amount,
            BalanceAfter = (latestBalance ?? 0) + amount,
            Note = note,
        };

        _ctx.Add(transaction);

        await publishEndpoint.Publish(
            new StockTransactionCreatedMessage
            {
                Resource = transaction,
                Branch = branchKey,
            },
            ct);

        return transaction;
    }

    public async Task<ResultObject<StockTransaction?>> RebalanceTransaction(
        BranchKey branchKey,
        IngredientKey ingredientKey,
        decimal targetAmount,
        string? note,
        CancellationToken ct = default)
    {
        var latestBalance = await GetLatestBalance(branchKey, ingredientKey, ct);

        if (targetAmount == latestBalance)
            return ResultObject.None();

        var transaction = new StockTransaction
        {
            RestaurantId = branchKey.RestaurantId,
            BranchId = branchKey.Id,
            Id = Guid.CreateVersion7(),
            IngredientId = ingredientKey.Id,
            Amount = targetAmount - (latestBalance ?? 0) ,
            BalanceAfter = targetAmount,
            Note = note,
        };

        _ctx.Add(transaction);

        await publishEndpoint.Publish(
            new StockTransactionCreatedMessage
            {
                Resource = transaction,
                Branch = branchKey,
            },
            ct);

        return transaction;
    }

    public IQueryable<StockTransaction> QueryTransactions()
    {
        return _ctx.Set<StockTransaction>()
            .AsExpandable();
    }

    public IQueryable<StockTransaction> QuerySingleTransaction(
        StockTransactionKey key)
    {
        return QueryTransactions()
            .Where(e =>
                e.RestaurantId == key.RestaurantId &&
                e.BranchId == key.BranchId &&
                e.Id == key.Id);
    }

    public IQueryable<StockTransaction> QueryIngredientTransactions(
        BranchKey branchKey, IngredientKey ingredientKey)
    {
        return QueryTransactions()
            .Where(e =>
                e.RestaurantId == branchKey.RestaurantId &&
                e.BranchId == branchKey.Id &&
                e.IngredientId == ingredientKey.Id)
            .OrderByDescending(e => e.Id);
    }
}