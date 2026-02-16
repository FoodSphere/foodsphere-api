namespace FoodSphere.SelfOrdering.Api.DTO;

public class RestaurantBranchResponse
{
    /// <example>ร้านบิ๊กบัง</example>
    public required string restaurant_name { get; set; }

    public string? branch_name { get; set; }

    /// <inheritdoc />
    public required ContactDto contact { get; set; }

    /// <example>71 Lat Krabang Rd, Lat Krabang, Bangkok 10520</example>
    public string? address { get; set; }

    /// <example>10:00</example>
    public TimeOnly? opening_time { get; set; }

    /// <example>22:00</example>
    public TimeOnly? closing_time { get; set; }

    public static readonly Func<Branch, RestaurantBranchResponse> Project = Projection.Compile();

    public static Expression<Func<Branch, RestaurantBranchResponse>> Projection =>
        model => new RestaurantBranchResponse
        {
            contact = ContactDto.Projection.Invoke(model.Contact ?? model.Restaurant.Contact),
            restaurant_name = model.Restaurant.DisplayName ?? model.Restaurant.Name,
            branch_name = model.DisplayName ?? model.Name,
            address = model.Address,
            opening_time = model.OpeningTime,
            closing_time = model.ClosingTime,
        };
}