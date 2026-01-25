namespace FoodSphere.Pos.Api.DTOs;

public class QuickRestaurantRequest
{
    public ContactDto? contact { get; set; }
    public required string name { get; set; }
    public string? display_name { get; set; }
    public string? address { get; set; }
    public TimeOnly? opening_time { get; set; }
    public TimeOnly? closing_time { get; set; }
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

public class QuickRestaurantResponse
{
    public Guid restaurant_id { get; set; }
    public ContactDto restaurant_contact { get; set; } = null!;
    public required string restaurant_name { get; set; }
    public string? restaurant_display_name { get; set; }

    public short branch_id { get; set; }
    public string? branch_name { get; set; }
    public string? branch_address { get; set; }
    public TimeOnly? branch_opening_time { get; set; }
    public TimeOnly? branch_closing_time { get; set; }

    public static QuickRestaurantResponse FromModel(Restaurant restaurant, Branch branch)
    {
        return new QuickRestaurantResponse
        {
            restaurant_id = restaurant.Id,
            restaurant_contact = ContactDto.FromModel(restaurant.Contact)!,
            restaurant_name = restaurant.Name,
            restaurant_display_name = restaurant.DisplayName,
            branch_id = branch.Id,
            branch_name = branch.Name,
            branch_address = branch.Address,
            branch_opening_time = branch.OpeningTime,
            branch_closing_time = branch.ClosingTime,
        };
    }
}