namespace FoodSphere.SelfOrdering.Api.DTOs;

public class SelfOrderingTokenRequest
{
    public Guid portal_id { get; set; }
    public string? consumer_token { get; set; }
}

public class SelfOrderingTokenResponse
{
    public required string access_token { get; set; }
}