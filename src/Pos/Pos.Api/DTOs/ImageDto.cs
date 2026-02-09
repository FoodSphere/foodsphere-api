namespace FoodSphere.Pos.Api.DTO;

public class UploadImageResponse
{
    public required string image_url { get; set; }
}

public class PresignUrlResponse
{
    public required string url { get; set; }
}