namespace FoodSphere.Pos.Api.DTO;

public class RestaurantRequest
{
    public ContactDto? contact { get; set; }

    /// <example>bigbang restaurant</example>
    public required string name { get; set; }

    /// <example>ร้านบิ๊กบัง</example>
    public string? display_name { get; set; }
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

public class SingleRestaurantRequest
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

public class SingleRestaurantResponse
{
    public Guid restaurant_id { get; set; }

    public required ContactDto contact { get; set; }

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

    public static SingleRestaurantResponse FromModel(Branch branch)
    {
        return new SingleRestaurantResponse
        {
            restaurant_id = branch.RestaurantId,
            contact = ContactDto.FromModel(branch.Restaurant.Contact)!,
            name = branch.Restaurant.Name,
            display_name = branch.Restaurant.DisplayName,
            address = branch.Address,
            opening_time = branch.OpeningTime,
            closing_time = branch.ClosingTime,
        };
    }
}