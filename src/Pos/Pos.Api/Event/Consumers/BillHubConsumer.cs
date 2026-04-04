using Microsoft.AspNetCore.SignalR;
using MassTransit;

namespace FoodSphere.Pos.Api.Event;

public class BillHubConsumer(
    ILogger<BillHubConsumer> logger,
    IHubContext<PosHub, IPosHub> hubContext,
    BillServiceBase billService
) : IConsumer<BillCreatedMessage>,
    IConsumer<BillStatusUpdatedMessage>
{
    public async Task Consume(
        ConsumeContext<BillCreatedMessage> context)
    {
        var msg = context.Message;
        var response = await billService.GetBill(
            BillResponse.Projection, msg.Resource);

        if (response is null)
        {
            logger.LogError(
                "{Keys} not found",
                    msg.Resource);
            return;
        }

        await hubContext.Clients
            .Group(msg.Branch)
            .bill_created(response);
    }

    public async Task Consume(
        ConsumeContext<BillStatusUpdatedMessage> context)
    {
        var msg = context.Message;
        var response = await billService.GetBill(
            e => new { e.TableId }, msg.Resource);

        if (response is null)
        {
            logger.LogError(
                "{Keys} not found",
                    msg.Resource);
            return;
        }

        await hubContext.Clients
            .Group(msg.Branch)
            .bill_status_updated(msg);
    }
}