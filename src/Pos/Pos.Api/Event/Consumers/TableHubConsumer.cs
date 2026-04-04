using Microsoft.AspNetCore.SignalR;
using MassTransit;

namespace FoodSphere.Pos.Api.Event;

public class TableHubConsumer(
    ILogger<TableHubConsumer> logger,
    IHubContext<PosHub, IPosHub> hubContext,
    TableServiceBase tableService
) : IConsumer<TableCreatedMessage>,
    IConsumer<TableStatusUpdatedMessage>
{
    public async Task Consume(
        ConsumeContext<TableCreatedMessage> context)
    {
        var msg = context.Message;
        var response = await tableService.GetTable(
            TableResponse.Projection,
            msg.Resource);

        if (response is null)
        {
            logger.LogError(
                "{Keys} not found",
                    msg.Resource);
            return;
        }

        await hubContext.Clients
            .Group(msg.Resource.RestaurantId, msg.Resource.BranchId)
            .table_created(response);
    }

    public async Task Consume(
        ConsumeContext<TableStatusUpdatedMessage> context)
    {
        var msg = context.Message;

        await hubContext.Clients
            .Group(msg.Resource.RestaurantId, msg.Resource.BranchId)
            .table_status_updated(msg);
    }
}