namespace FoodSphere.SelfOrdering.Api.DTO;

public record SelfOrderingTokenRequest
{
    public required Guid portal_id { get; set; }
    public string? consumer_token { get; set; }
}

public record SelfOrderingTokenResponse
{
    public required string access_token { get; set; }
}