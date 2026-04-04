using Microsoft.AspNetCore.SignalR;
using MassTransit;

namespace FoodSphere.Pos.Api.Event;

public class StockHubConsumer(
    ILogger<StockHubConsumer> logger,
    IHubContext<PosHub, IPosHub> hubContext,
    StockServiceBase stockService
) : IConsumer<StockTransactionCreatedMessage>
{
    public async Task Consume(
        ConsumeContext<StockTransactionCreatedMessage> context)
    {
        var msg = context.Message;
        var response = await stockService.GetTransaction(
            StockTransactionResponse.Projection,
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
            .stock_transaction_created(response);
    }
}