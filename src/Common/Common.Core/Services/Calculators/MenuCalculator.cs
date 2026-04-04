namespace FoodSphere.Common.Service;

public class MenuCalculator(
    OrderRepository orderRepository,
    PaymentRepository paymentRepository,
    BillRepository billRepository,
    StockTransactionRepository stockRepository
) : ServiceBase
{
    // public async Task<bool> GetStockAvailability(
    //     MenuKey menuKey,
    //     CancellationToken ct = default)
    // {

    //     return;
    // }
}