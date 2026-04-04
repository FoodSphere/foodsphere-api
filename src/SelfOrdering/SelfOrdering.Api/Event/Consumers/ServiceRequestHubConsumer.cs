using Microsoft.AspNetCore.SignalR;
using MassTransit;

namespace FoodSphere.SelfOrdering.Api.Event;

public class ServiceRequestHubConsumer(
    ILogger<ServiceRequestHubConsumer> logger,
    IHubContext<OrderingHub, IOrderingHub> hubContext,
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
            .Group(msg.Resource.BillId)
            .service_request_created(response);
    }

    public async Task Consume(
        ConsumeContext<ServiceRequestStatusUpdatedMessage> context)
    {
        var msg = context.Message;

        await hubContext.Clients
            .Group(msg.Resource.BillId)
            .service_request_status_updated(msg);
    }
}