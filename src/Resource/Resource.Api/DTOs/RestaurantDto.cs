namespace FoodSphere.Resource.Api.DTOs;

public class RestaurantRequest
{
    public required string owner_id { get; set; }
    public ContactDto? contact { get; set; }
    public required string name { get; set; }
    public string? display_name { get; set; }
}

public class RestaurantResponse
{
    public Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public ContactDto contact { get; set; } = null!;

    public required string name { get; set; }
    public string? display_name { get; set; }

    public static RestaurantResponse FromModel(Restaurant model)
    {
        return new RestaurantResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            contact = ContactDto.FromModel(model.Contact)!,
            name = model.Name,
            display_name = model.DisplayName,
        };
    }
}