using Microsoft.AspNetCore.SignalR;
using MassTransit;

namespace FoodSphere.SelfOrdering.Api.Event;

public class PaymentHubConsumer(
    ILogger<PaymentHubConsumer> logger,
    IHubContext<OrderingHub, IOrderingHub> hubContext,
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
            .Group(msg.Resource.BillId)
            .payment_created(response);
    }

    public async Task Consume(
        ConsumeContext<PaymentStatusUpdatedMessage> context)
    {
        var msg = context.Message;

        await hubContext.Clients
            .Group(msg.Resource.BillId)
            .payment_status_updated(msg);
    }
}