namespace FoodSphere.Pos.Api.DTO;

public class QuickRestaurantRequest
{
    public ContactDto? contact { get; set; }

    /// <example>bigbang restaurant</example>
    public required string name { get; set; }

    /// <example>ร้านบิ๊กบัง</example>
    public string? display_name { get; set; }

    /// <example>71 Lat Krabang Rd, Lat Krabang, Bangkok 10520</example>
    public string? address { get; set; }

    /// <example>10:00</example>
    public TimeOnly? opening_time { get; set; }

    /// <example>22:00</example>
    public TimeOnly? closing_time { get; set; }
}

public class RestaurantResponse
{
    public Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public required ContactDto contact { get; set; }

    /// <example>super restaurant</example>
    public required string name { get; set; }

    /// <example>ร้านบิ๊กบัง</example>
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

    /// <inheritdoc />
    public required ContactDto restaurant_contact { get; set; }

    /// <example>bigbang restaurant</example>
    public required string restaurant_name { get; set; }

    /// <example>ร้านบิ๊กบัง</example>
    public string? restaurant_display_name { get; set; }

    public short branch_id { get; set; }

    /// <example>main</example>
    public required string branch_name { get; set; }

    /// <example>null</example>
    public string? branch_display_name { get; set; }

    /// <example>71 Lat Krabang Rd, Lat Krabang, Bangkok 10520</example>
    public string? branch_address { get; set; }

    /// <example>10:00</example>
    public TimeOnly? branch_opening_time { get; set; }

    /// <example>22:00</example>
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
            branch_display_name = branch.DisplayName,
            branch_address = branch.Address,
            branch_opening_time = branch.OpeningTime,
            branch_closing_time = branch.ClosingTime,
        };
    }
}