namespace FoodSphere.Pos.Api.DTOs;

public class StaffTokenRequest
{
    public Guid portal_id { get; set; }
}

public class StaffTokenResponse
{
    public required string access_token { get; set; }
}