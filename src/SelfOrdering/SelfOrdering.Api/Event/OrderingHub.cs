using Microsoft.AspNetCore.SignalR;

namespace FoodSphere.SelfOrdering.Api.Event;

public interface IOrderingHub
{
    Task bill_status_updated(BillStatusUpdatedMessage message);

    Task order_created(OrderResponse message);
    Task order_status_updated(OrderStatusUpdatedMessage message);
    Task order_item_updated(OrderItemResponse message);

    Task payment_created(PaymentResponse message);
    Task payment_status_updated(PaymentStatusUpdatedMessage message);

    Task service_request_created(ServiceRequestResponse message);
    Task service_request_status_updated(ServiceRequestStatusUpdatedMessage message);
}

[OrderingAuthorize]
public class OrderingHub(
    ILogger<OrderingHub> logger
) : Hub<IOrderingHub>
{
    public override async Task OnConnectedAsync()
    {
        if (Context.GetHttpContext() is not HttpContext httpContext)
        {
            Context.Abort();
            return;
        }

        var member = (httpContext.Items[nameof(BillMemberKey)] as BillMemberKey)!;

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            HubHelper.GetGroupName(member.BillId));

        await base.OnConnectedAsync();
    }
}