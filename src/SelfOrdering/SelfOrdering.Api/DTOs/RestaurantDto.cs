namespace FoodSphere.SelfOrdering.Api.DTO;

public record RestaurantBranchResponse
{
    /// <example>ร้านบิ๊กบัง</example>
    public required string restaurant_name { get; set; }
    public required Guid restaurant_id { get; set; }

    public string? branch_name { get; set; }
    public required short branch_id { get; set; }

    /// <inheritdoc />
    public required ContactDto contact { get; set; }

    public string? stripe_account_id { get; set; }

    /// <example>71 Lat Krabang Rd, Lat Krabang, Bangkok 10520</example>
    public string? address { get; set; }

    /// <example>10:00</example>
    public TimeOnly? opening_time { get; set; }

    /// <example>22:00</example>
    public TimeOnly? closing_time { get; set; }

    public static readonly Expression<Func<Branch, RestaurantBranchResponse>> Projection =
        model => new()
        {
            contact = model.Contact != null ?
                ContactDto.Projection.Invoke(model.Contact) :
                ContactDto.Projection.Invoke(model.Restaurant.Contact),
            restaurant_name = model.Restaurant.DisplayName ?? model.Restaurant.Name,
            restaurant_id = model.RestaurantId,
            branch_name = model.DisplayName ?? model.Name,
            branch_id = model.Id,
            stripe_account_id = model.Restaurant.StripeAccountId,
            address = model.Address,
            opening_time = model.OpeningTime,
            closing_time = model.ClosingTime,
        };

    public static readonly Func<Branch, RestaurantBranchResponse> Project = Projection.Compile();
}