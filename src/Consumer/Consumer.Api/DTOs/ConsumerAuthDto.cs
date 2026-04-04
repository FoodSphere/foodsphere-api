namespace FoodSphere.Consumer.Api.DTO;

public record ConsumerTokenRequest
{
    public required string username { get; set; }
    public required string password { get; set; }
}

public record ConsumerTokenResponse
{
    public required string access_token { get; set; }
}