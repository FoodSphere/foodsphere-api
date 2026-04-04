namespace FoodSphere.Pos.Api.DTO;

public record RestaurantRequest
{
    public ContactDto? contact { get; set; }

    /// <example>bigbang restaurant</example>
    public required string name { get; set; }

    /// <example>ร้านบิ๊กบัง</example>
    public string? display_name { get; set; }
}

public record RestaurantResponse
{
    public required Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }
    public DateTime? delete_time { get; set; }

    public required ContactDto contact { get; set; }

    /// <example>super restaurant</example>
    public required string name { get; set; }

    public string? stripe_account_id { get; set; }

    /// <example>ร้านบิ๊กบัง</example>
    public string? display_name { get; set; }

    public string? image_url { get; set; }

    public static readonly Expression<Func<Restaurant, RestaurantResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            delete_time = model.DeleteTime,
            contact = ContactDto.Projection.Invoke(model.Contact),
            name = model.Name,
            stripe_account_id = model.StripeAccountId,
            display_name = model.DisplayName,
            image_url = model.ImageUrl
        };

    public static readonly Func<Restaurant, RestaurantResponse> Project = Projection.Compile();
}

public record SingleRestaurantRequest
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

public record SingleRestaurantResponse
{
    public required Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }
    public DateTime? delete_time { get; set; }

    public required ContactDto contact { get; set; }

    /// <example>bigbang restaurant</example>
    public required string name { get; set; }

    public string? stripe_account_id { get; set; }

    /// <example>ร้านบิ๊กบัง</example>
    public string? display_name { get; set; }

    public string? image_url { get; set; }

    /// <example>71 Lat Krabang Rd, Lat Krabang, Bangkok 10520</example>
    public string? address { get; set; }

    /// <example>10:00</example>
    public TimeOnly? opening_time { get; set; }

    /// <example>22:00</example>
    public TimeOnly? closing_time { get; set; }

    public static readonly Expression<Func<Branch, SingleRestaurantResponse>> Projection =
        model => new()
        {
            id = model.RestaurantId,
            create_time = model.Restaurant.CreateTime,
            update_time = model.Restaurant.UpdateTime,
            delete_time = model.Restaurant.DeleteTime,
            contact = ContactDto.Projection.Invoke(model.Restaurant.Contact),
            name = model.Restaurant.Name,
            stripe_account_id = model.Restaurant.StripeAccountId,
            display_name = model.Restaurant.DisplayName,
            image_url = model.Restaurant.ImageUrl,
            address = model.Address,
            opening_time = model.OpeningTime,
            closing_time = model.ClosingTime,
        };

    public static readonly Func<Branch, SingleRestaurantResponse> Project = Projection.Compile();
}