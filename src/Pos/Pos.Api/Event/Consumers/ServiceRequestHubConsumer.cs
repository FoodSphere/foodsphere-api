using Microsoft.AspNetCore.SignalR;
using MassTransit;

namespace FoodSphere.Pos.Api.Event;

public class ServiceRequestHubConsumer(
    ILogger<ServiceRequestHubConsumer> logger,
    IHubContext<PosHub, IPosHub> hubContext,
    ServiceRequestService serviceRequestService
) : IConsumer<ServiceRequestCreatedMessage>,
    IConsumer<ServiceRequestStatusUpdatedMessage>
{
    public async Task Consume(
        ConsumeContext<ServiceRequestCreatedMessage> context)
    {
        var msg = context.Message;
        var response = await serviceRequestService.GetRequest(
            ServiceRequestResponse.Projection,
            msg.Resource);

        if (response is null)
        {
            logger.LogError(
                "{Keys} not found",
                    msg.Resource);
            return;
        }

        await hubContext.Clients
            .Group(msg.Table.RestaurantId, msg.Table.BranchId)
            .service_request_created(response);
    }

    public async Task Consume(
        ConsumeContext<ServiceRequestStatusUpdatedMessage> context)
    {
        var msg = context.Message;
        var branchKey = await serviceRequestService.GetRequest(
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
            .service_request_status_updated(msg);
    }
}