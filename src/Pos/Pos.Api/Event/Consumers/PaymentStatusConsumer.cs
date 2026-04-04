using FoodSphere.Infrastructure.Repository;
using MassTransit;

namespace FoodSphere.Pos.Api.Event;

public class PaymentStatusConsumer(
    ILogger<PaymentStatusConsumer> logger,
    IPublishEndpoint publishEndpoint,
    PersistenceService persistenceService,
    BillRepository billRepository
) : IConsumer<PaymentStatusUpdatedMessage>
{
    public async Task Consume(
        ConsumeContext<PaymentStatusUpdatedMessage> context)
    {
        var msg = context.Message;

        if (msg.Status is not PaymentStatus.Succeeded)
            return;

        var bill = await billRepository.GetBill(
            new(msg.Resource.BillId), context.CancellationToken);

        if (bill is null)
        {
            logger.LogError(
                "Branch for {Keys} not found",
                    msg.Resource);
            return;
        }

        bill.Status = BillStatus.Paid;

        await context.Publish(
            new BillStatusUpdatedMessage
            {
                Resource = bill,
                Status = bill.Status,
                Branch = new(bill.RestaurantId, bill.BranchId)
            });

        await persistenceService.Commit(context.CancellationToken);
    }
}