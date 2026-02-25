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

    public static RestaurantBranchResponse FromModel(Restaurant restaurant, Branch branch)
    {
        return new RestaurantBranchResponse
        {
            contact = ContactDto.FromModel(branch.Contact ?? restaurant.Contact)!,
            restaurant_name = restaurant.DisplayName ?? restaurant.Name,
            branch_name = branch.DisplayName ?? branch.Name,
            address = branch.Address,
            opening_time = branch.OpeningTime,
            closing_time = branch.ClosingTime,
        };
    }
}