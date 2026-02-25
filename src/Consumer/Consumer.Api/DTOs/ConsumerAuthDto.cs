namespace FoodSphere.Consumer.Api.DTO;

public class ConsumerTokenRequest
{
    public required string username { get; set; }
    public required string password { get; set; }
}

public class ConsumerTokenResponse
{
    public required string access_token { get; set; }
}