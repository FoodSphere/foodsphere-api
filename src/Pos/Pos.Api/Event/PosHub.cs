using Microsoft.AspNetCore.SignalR;

namespace FoodSphere.Pos.Api.Event;

public interface IPosHub
{
    Task bill_created(BillResponse message);
    Task bill_status_updated(BillStatusUpdatedMessage message);

    Task order_created(OrderResponse message);
    Task order_status_updated(OrderStatusUpdatedMessage message);
    Task order_item_updated(OrderItemResponse message);

    Task payment_created(PaymentResponse message);
    Task payment_status_updated(PaymentStatusUpdatedMessage message);

    Task service_request_created(ServiceRequestResponse message);
    Task service_request_status_updated(ServiceRequestStatusUpdatedMessage message);

    Task stock_transaction_created(StockTransactionResponse message);

    Task table_created(TableResponse message);
    Task table_status_updated(TableStatusUpdatedMessage message);
}

[PosAuthorize]
public class PosHub(
    ILogger<PosHub> logger,
    AccessControlService accessControl
) : Hub<IPosHub>
{
    public override async Task OnConnectedAsync()
    {
        if (Context.GetHttpContext() is not HttpContext httpContext)
        {
            Context.Abort();
            return;
        }

        var routeValues = httpContext.Request.RouteValues;

        logger.LogInformation("Path: {path}, RouteValues: {routeValues}",
            httpContext.Request.Path.Value,
            string.Join(", ", routeValues.Select(kv => $"{kv.Key}={kv.Value}")));

        if (httpContext.GetRouteValue("restaurant_id") is not string restaurant_id)
        {
            logger.LogError("Restaurant ID is missing in the route");
            Context.Abort();
            return;
        }

        var branch_id = httpContext.GetRouteValue("branch_id") as string;

        var authorizeResult = await accessControl.Authorize(httpContext);

        if (authorizeResult.IsFailed)
        {
            Context.Abort();
            return;
        }

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            HubHelper.GetGroupName(restaurant_id, branch_id));

        await base.OnConnectedAsync();
    }
}