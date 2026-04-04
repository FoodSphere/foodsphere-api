using Microsoft.AspNetCore.SignalR;

namespace FoodSphere.SelfOrdering.Api.Event;

public static class HubHelper
{
    public static string GetGroupName(Guid bill_id)
    {
        return $"{bill_id}";
    }

    extension(IHubClients<IOrderingHub> hubClients)
    {
        public IOrderingHub Group(Guid bill_id)
        {
            return hubClients.Group($"{bill_id}");
        }

        public IOrderingHub Group(BillKey billKey)
        {
            return hubClients.Group($"{billKey.Id}");
        }
    }
}