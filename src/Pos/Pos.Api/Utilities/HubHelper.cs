using Microsoft.AspNetCore.SignalR;

namespace FoodSphere.Pos.Api.Event;

public static class HubHelper
{
    public static string GetGroupName(string restaurant_id, string? branch_id = null)
    {
        return $"{restaurant_id}-{branch_id ?? "1"}";
    }

    extension(IHubClients<IPosHub> hubClients)
    {
        public IPosHub Group(Guid restaurant_id, short branch_id)
        {
            return hubClients.Group($"{restaurant_id}-{branch_id}");
        }

        public IPosHub Group(BranchKey key)
        {
            return hubClients.Group($"{key.RestaurantId}-{key.Id}");
        }
    }
}