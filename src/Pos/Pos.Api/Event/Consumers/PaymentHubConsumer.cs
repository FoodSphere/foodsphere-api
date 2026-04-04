using Microsoft.AspNetCore.SignalR;
using MassTransit;

namespace FoodSphere.Pos.Api.Event;

public class PaymentHubConsumer(
    ILogger<PaymentHubConsumer> logger,
    IHubContext<PosHub, IPosHub> hubContext,
    PaymentService paymentService
) : IConsumer<PaymentCreatedMessage>,
    IConsumer<PaymentStatusUpdatedMessage>
{
    public async Task Consume(
        ConsumeContext<PaymentCreatedMessage> context)
    {
        var msg = context.Message;
        var response = await paymentService.GetPayment(
            PaymentResponse.Projection,
            msg.Resource);

        if (response is null)
        {
            logger.LogError(
                "{Keys} not found",
                    msg.Resource);
            return;
        }

        await hubContext.Clients
            .Group(msg.Branch)
            .payment_created(response);
    }

    public async Task Consume(
        ConsumeContext<PaymentStatusUpdatedMessage> context)
    {
        var msg = context.Message;
        var branchKey = await paymentService.GetPayment(
            e => new BranchKey(e.Bill.RestaurantId, e.Bill.BranchId),
            msg.Resource);

        if (branchKey is null)
        {
            logger.LogError(
                "Branch for {Keys} not found",
                    msg.Resource);
            return;
        }

        await hubContext.Clients
            .Group(branchKey)
            .payment_status_updated(msg);
    }
}