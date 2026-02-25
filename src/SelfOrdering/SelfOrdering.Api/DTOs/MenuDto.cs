namespace FoodSphere.SelfOrdering.Api.DTO;

public class MenuResponse
{
    public int id { get; set; }

    /// <example>ข้าวผัดโคขุน</example>
    public required string name { get; set; }

    /// <example>120</example>
    public int price { get; set; }

    /// <example>ข้าวผัดโคขุนสูตรเด็ดของทางร้าน</example>
    public string? display_name { get; set; }

    /// <example>เนื้อโคจากออสเตรเลีย</example>
    public string? description { get; set; }

    public string? image_url { get; set; }

    public MenuStatus status { get; set; }

    public static MenuResponse FromModel(Menu model)
    {
        return new MenuResponse
        {
            id = model.Id,
            name = model.Name,
            display_name = model.DisplayName,
            description = model.Description,
            image_url = model.ImageUrl,
            price = model.Price,
            status = model.Status,
        };
    }
}