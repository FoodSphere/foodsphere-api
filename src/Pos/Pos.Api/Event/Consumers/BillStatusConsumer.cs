using FoodSphere.Infrastructure.Repository;
using MassTransit;

namespace FoodSphere.Pos.Api.Event;

public class BillStatusConsumer(
    ILogger<BillStatusConsumer> logger,
    PersistenceService persistenceService,
    BillServiceBase billService,
    TableRepository tableRepository
) : IConsumer<BillStatusUpdatedMessage>
{
    public async Task Consume(
        ConsumeContext<BillStatusUpdatedMessage> context)
    {
        var msg = context.Message;

        if (msg.Status is not BillStatus.Completed)
            return;

        var bill = await billService.GetBill(
            e => new { e.Table }, msg.Resource,
            context.CancellationToken);

        if (bill is null)
        {
            logger.LogError(
                "Table for {Keys} not found",
                    msg.Resource);
            return;
        }

        var tableStatusResult = await tableRepository.UpdateTableStatus(
            bill.Table, TableStatus.Ready);

        await persistenceService.Commit();
    }
}