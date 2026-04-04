using Microsoft.AspNetCore.SignalR;
using MassTransit;

namespace FoodSphere.Pos.Api.Event;

public class OrderHubConsumer(
    ILogger<OrderHubConsumer> logger,
    IHubContext<PosHub, IPosHub> hubContext,
    OrderServiceBase orderService,
    BillServiceBase billService
) : IConsumer<OrderCreatedMessage>,
    IConsumer<OrderStatusUpdatedMessage>,
    IConsumer<OrderItemUpdatedMessage>
{
    public async Task Consume(
        ConsumeContext<OrderCreatedMessage> context)
    {
        var msg = context.Message;
        var response = await orderService.GetOrder(
            OrderResponse.Projection,
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
            .order_created(response);
    }

    public async Task Consume(
        ConsumeContext<OrderStatusUpdatedMessage> context)
    {
        var msg = context.Message;
        var branchKey = await billService.GetBill(
            e => new BranchKey(e.RestaurantId, e.BranchId),
            new(msg.Resource.BillId));

        if (branchKey is null)
        {
            logger.LogError(
                "Branch for {Keys} not found",
                    msg.Resource);
            return;
        }

        await hubContext.Clients
            .Group(branchKey)
            .order_status_updated(msg);
    }

    public async Task Consume(ConsumeContext<OrderItemUpdatedMessage> context)
    {
        var msg = context.Message;
        var branchKey = await billService.GetBill(
            e => new BranchKey(e.RestaurantId, e.BranchId),
            new(msg.Resource.BillId));

        if (branchKey is null)
        {
            logger.LogError(
                "Branch for {Keys} not found",
                    msg.Resource);
            return;
        }

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
            .Group(branchKey)
            .order_item_updated(response);
    }
}