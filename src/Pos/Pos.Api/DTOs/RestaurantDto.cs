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

    public static readonly Func<Restaurant, RestaurantResponse> Project = Projection.Compile();

    public static Expression<Func<Restaurant, RestaurantResponse>> Projection =>
        model => new RestaurantResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            contact = ContactDto.Projection.Invoke(model.Contact),
            name = model.Name,
            display_name = model.DisplayName,
        };
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

    public static readonly Func<Branch, SingleRestaurantResponse> Project = Projection.Compile();

    public static Expression<Func<Branch, SingleRestaurantResponse>> Projection =>
        model => new SingleRestaurantResponse
        {
            restaurant_id = model.RestaurantId,
            contact = ContactDto.Projection.Invoke(model.Restaurant.Contact),
            name = model.Restaurant.Name,
            display_name = model.Restaurant.DisplayName,
            address = model.Address,
            opening_time = model.OpeningTime,
            closing_time = model.ClosingTime,
        };
}