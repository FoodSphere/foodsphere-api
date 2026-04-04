namespace FoodSphere.Common.Service;

public class OrderingCalculator(
    ILogger<OrderingCalculator> logger,
    OrderRepository orderRepository,
    OrderServiceBase orderService,
    StockTransactionRepository stockRepository,
    MenuRepository menuRepository
) : ServiceBase
{
    public async Task<decimal> CalculateBillTotalAmount(
        BillKey billKey, CancellationToken ct = default)
    {
        var total = await orderRepository.QueryItems()
            .Where(e =>
                e.BillId == billKey.Id && (
                    e.Order.Status == OrderStatus.Served ||
                    e.Order.Status == OrderStatus.Cooking ||
                    e.Order.Status == OrderStatus.Pending))
            .SumAsync(item =>
                item.PriceSnapshot * item.Quantity,
                ct);

        return total;
    }

    public async Task<ResultObject> CalculateStockAvailability(
        Dictionary<MenuKey, short> items, BranchKey branchKey,
        CancellationToken ct = default)
    {
        var requestedItems = items.ToDictionary(
            e => e.Key.Id,
            e => e.Value);

        var menuIds = requestedItems.Keys.ToArray();

        var menuIngredients = await menuRepository.QueryMenuIngredients()
            .Where(e =>
                e.RestaurantId == branchKey.RestaurantId &&
                menuIds.Contains(e.MenuId))
            .GroupBy(e => e.MenuId)
            .Select(g => new
            {
                MenuId = g.Key,
                Ingredients = g.Select(e => new
                {
                    e.IngredientId,
                    e.Amount,
                }).ToArray(),
            })
            .ToDictionaryAsync(
                e => e.MenuId,
                e => e.Ingredients,
                ct);

        var ingredientUsed = menuIngredients
            .SelectMany(e => e.Value.Select(i => new
            {
                i.IngredientId,
                Amount = i.Amount * requestedItems[e.Key],
                MenuId = e.Key,
            }))
            .GroupBy(e => e.IngredientId)
            .Select(g => new
            {
                IngredientId = g.Key,
                Amount = g.Sum(e => e.Amount),
                MenuIds = g.Select(e => e.MenuId).ToArray(),
            })
            .ToDictionary(
                e => e.IngredientId,
                e => new { e.Amount, e.MenuIds });

        var ingredientIds = ingredientUsed.Keys.ToArray();

        var remainingStock = await stockRepository.QueryTransactions()
            .Where(e =>
                e.RestaurantId == branchKey.RestaurantId &&
                e.BranchId == branchKey.Id &&
                ingredientIds.Contains(e.IngredientId))
            .GroupBy(e => e.IngredientId)
            .Select(g => g
                .OrderByDescending(e => e.Id)
                .First())
            .ToDictionaryAsync(
                e => e.IngredientId,
                e => e.BalanceAfter,
                ct);

        var reservedStock = await orderRepository.QueryItems()
            .Where(item =>
                item.Bill.RestaurantId == branchKey.RestaurantId &&
                item.Bill.BranchId == branchKey.Id &&
                item.Order.Status == OrderStatus.Pending)
            .SelectMany(item => item.Menu.Ingredients
                .Where(e =>
                    ingredientIds.Contains(e.IngredientId))
                .Select(e => new
                {
                    e.IngredientId,
                    Amount = item.Quantity * e.Amount,
                }))
            .GroupBy(e => e.IngredientId)
            .Select(g => new
            {
                IngredientId = g.Key,
                Amount = g.Sum(e => e.Amount),
            })
            .ToDictionaryAsync(
                e => e.IngredientId,
                e => e.Amount,
                ct);

        var missingIngredientIds = ingredientUsed.Keys.Except(remainingStock.Keys);

        if (missingIngredientIds.Any())
            return ResultObject.Fail(ResultError.NotFound,
                "Ingredients not found", new { ingredient_ids = missingIngredientIds });

        var insufficients = new HashSet<short>();

        foreach (var used in ingredientUsed) {
            var available = remainingStock[used.Key] - reservedStock.GetValueOrDefault(used.Key, 0m);
            logger.LogInformation(
                "Ingredient {IngredientId}: Remaining {Remaining}, Reserved {Reserved}, Used {Used}",
                used.Key, remainingStock[used.Key], reservedStock.GetValueOrDefault(used.Key, 0m), used.Value.Amount);

            if (available < used.Value.Amount)
            {
                foreach (var menuId in used.Value.MenuIds)
                {
                    logger.LogInformation(
                        "Menu {MenuId} contributes to insufficient stock of Ingredient {IngredientId}",
                        menuId, used.Key);
                    insufficients.Add(menuId);
                }
            }
        }

        if (insufficients.Count != 0)
            return ResultObject.Fail(ResultError.State,
                "Insufficient stock for some menu items",
                new { menu_keys = insufficients });

        return ResultObject.Success();
    }
}