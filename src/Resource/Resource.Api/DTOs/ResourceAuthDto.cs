namespace FoodSphere.Resource.Api.DTOs;

public class ResourceTokenRequest
{
    public required string identifier { get; set; }
}

public class ResourceTokenResponse
{
    public required string access_token { get; set; }
}