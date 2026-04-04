namespace FoodSphere.Common.Service;

public class ReportCalculator(
    OrderRepository orderRepository,
    PaymentRepository paymentRepository,
    BillRepository billRepository,
    StockTransactionRepository stockRepository
) : ServiceBase
{
    public async Task<decimal> GetRevenue(
        RestaurantKey restaurantKey,
        DateTime? fromTime, DateTime? toTime,
        CancellationToken ct = default)
    {
        var query = paymentRepository.QueryPayments()
            .Where(e =>
                e.Bill.RestaurantId == restaurantKey.Id &&
                e.Status == PaymentStatus.Succeeded);

        if (fromTime is not null)
            query = query.Where(e => e.CreateTime >= fromTime.Value);

        if (toTime is not null)
            query = query.Where(e => e.CreateTime <= toTime.Value);

        return await query
            .SumAsync(e => e.Amount, ct);
    }

    public async Task<int> CountMenuSold(
        Expression<Func<OrderItem, bool>> predicate,
        CancellationToken ct = default)
    {
        return await orderRepository.QueryItems()
            .Where(predicate)
            .SumAsync(e => e.Quantity, ct);
    }

    public async Task<int> CountBill(
        Expression<Func<Bill, bool>> predicate,
        CancellationToken ct = default)
    {
        return await billRepository.QueryBills()
            .Where(predicate)
            .CountAsync(ct);
    }

    public async Task<MenuSoldItemResponse[]> ListMenuSold(
        Expression<Func<OrderItem, bool>> predicate,
        CancellationToken ct = default)
    {
        return await orderRepository.QueryItems()
            .Where(predicate)
            .GroupBy(e => e.MenuId)
            .Select(g => new MenuSoldItemResponse
            {
                menu_id = g.Key,
                menu_name = g.First().Menu.Name,
                total_sold = g.Sum(e => e.Quantity),
            })
            .ToArrayAsync(ct);
    }

    public async Task<StockUsageResponse[]> ListStockUsage(
        RestaurantKey restaurantKey,
        DateTime? fromTime, DateTime? toTime,
        CancellationToken ct = default)
    {
        var query = stockRepository.QueryTransactions()
            .Where(e =>
                e.RestaurantId == restaurantKey.Id);

        if (fromTime is not null)
            query = query.Where(e => e.CreateTime >= fromTime.Value);

        if (toTime is not null)
            query = query.Where(e => e.CreateTime <= toTime.Value);

        var stockUsage = await query
            .GroupBy(e => new { e.IngredientId, e.Ingredient.Name })
            .Select(g => new StockUsageResponse
            {
                ingredient_id = g.Key.IngredientId,
                ingredient_name = g.Key.Name,
                total_used = g.Sum(e => e.Amount < 0 ? -e.Amount : 0),
                total_added = g.Sum(e => e.Amount > 0 ? e.Amount : 0),
            })
            .ToArrayAsync(ct);

        return stockUsage;
    }
}

public record MenuSoldItemResponse
{
    public short menu_id { get; set; }
    public required string menu_name { get; set; }
    public int total_sold { get; set; }
}

public record StockUsageResponse
{
    public short ingredient_id { get; set; }
    public required string ingredient_name { get; set; }
    public decimal total_used { get; set; }
    public decimal total_added { get; set; }
}