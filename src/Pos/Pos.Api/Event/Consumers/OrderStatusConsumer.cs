using FoodSphere.Infrastructure.Repository;
using MassTransit;

namespace FoodSphere.Pos.Api.Event;

public class OrderStatusConsumer(
    ILogger<OrderStatusConsumer> logger,
    PersistenceService persistenceService,
    OrderRepository orderRepository,
    OrderServiceBase orderService,
    StockTransactionRepository stockRepository
) : IConsumer<OrderStatusUpdatedMessage>
{
    public async Task Consume(
        ConsumeContext<OrderStatusUpdatedMessage> context)
    {
        var msg = context.Message;

        if (msg.Status is not OrderStatus.Cooking)
            return;

        var branchKey = await orderService.GetOrder(
            e => new BranchKey(e.Bill.RestaurantId, e.Bill.BranchId),
            msg.Resource);

        if (branchKey is null)
        {
            logger.LogError(
                "Branch for {Keys} not found",
                    msg.Resource);
            return;
        }

        var ingredientUsedMap = await orderRepository.QueryItems()
            .Where(e =>
                e.BillId == msg.Resource.BillId &&
                e.OrderId == msg.Resource.Id)
            .SelectMany(item => item.Menu.Ingredients
                .Select(i => new
                {
                    ItemId = item.Id,
                    i.IngredientId,
                    Amount = item.Quantity * i.Amount,
                })
                .Concat(item.Menu.Components
                    .SelectMany(c => c.ChildMenu.Ingredients
                        .Select(i => new
                        {
                            ItemId = item.Id,
                            i.IngredientId,
                            Amount = item.Quantity * c.Quantity * i.Amount,
                        }))))
            .ToArrayAsync(context.CancellationToken);

        foreach (var used in ingredientUsedMap)
            await stockRepository.CreateTransaction(
                branchKey: branchKey,
                ingredientKey: new(branchKey.RestaurantId, used.IngredientId),
                itemKey: new(msg.Resource.BillId, msg.Resource.Id, used.ItemId),
                amount: -used.Amount,
                note: null,
                ct: context.CancellationToken);

        await persistenceService.Commit();
    }
}