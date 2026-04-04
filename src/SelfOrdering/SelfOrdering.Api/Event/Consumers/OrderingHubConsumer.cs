using Microsoft.AspNetCore.SignalR;
using MassTransit;

namespace FoodSphere.SelfOrdering.Api.Event;

public class OrderingHubConsumer(
    ILogger<OrderingHubConsumer> logger,
    IHubContext<OrderingHub, IOrderingHub> hubContext,
    BillServiceBase billService,
    OrderServiceBase orderService
) : IConsumer<BillStatusUpdatedMessage>,
    IConsumer<OrderCreatedMessage>,
    IConsumer<OrderStatusUpdatedMessage>,
    IConsumer<OrderItemUpdatedMessage>
{
    public async Task Consume(ConsumeContext<BillStatusUpdatedMessage> context)
    {
        var msg = context.Message;

        await hubContext.Clients
            .Group(msg.Resource)
            .bill_status_updated(msg);
    }

    public async Task Consume(ConsumeContext<OrderCreatedMessage> context)
    {
        var msg = context.Message;
        var response = await orderService.GetOrder(
            OrderResponse.Projection,
            msg.Resource);

        if (response is null)
        {
            logger.LogError(
                "Order #{OrderId} of bill {BillId} not found",
                msg.Resource.Id, msg.Resource.BillId);
            return;
        }

        await hubContext.Clients
            .Group(msg.Resource.BillId)
            .order_created(response);
    }

    public async Task Consume(ConsumeContext<OrderStatusUpdatedMessage> context)
    {
        var msg = context.Message;

        await hubContext.Clients
            .Group(msg.Resource.BillId)
            .order_status_updated(msg);
    }

    public async Task Consume(ConsumeContext<OrderItemUpdatedMessage> context)
    {
        var msg = context.Message;
        var response = await orderService.GetItem(
            OrderItemResponse.Projection,
            msg.Resource);

        if (response is null)
        {
            logger.LogError(
                "Order item {OrderItemId} not found",
                msg.Resource);
            return;
        }

        await hubContext.Clients
            .Group(msg.Resource.BillId)
            .order_item_updated(response);
    }
}