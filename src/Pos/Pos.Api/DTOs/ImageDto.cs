namespace FoodSphere.Pos.Api.DTO;

public record UploadImageResponse
{
    public required string image_url { get; set; }
}

public record PresignUrlResponse
{
    public required string url { get; set; }
}