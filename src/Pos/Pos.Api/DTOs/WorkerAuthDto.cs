namespace FoodSphere.Pos.Api.DTO;

public record WorkerTokenRequest
{
    public required Guid portal_id { get; set; }
}

public record WorkerTokenResponse
{
    public required string access_token { get; set; }
}