namespace FoodSphere.Pos.Api.DTO;

public record BranchRequest
{
    public ContactDto? contact { get; set; }

    /// <example>โรบินสันลาดกระบัง</example>
    public required string name { get; set; }

    /// <example>ร้านบิ๊กบัง สาขาโรบินสันลาดกระบัง</example>
    public string? display_name { get; set; }

    /// <example>71 Lat Krabang Rd, Lat Krabang, Bangkok 10520</example>
    public string? address { get; set; }

    /// <example>10:00</example>
    public TimeOnly? opening_time { get; set; }

    /// <example>22:00</example>
    public TimeOnly? closing_time { get; set; }
}

public record BranchResponse //: IDTO<Branch, BranchResponse>
{
    public required int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }
    public DateTime? delete_time { get; set; }

    public required Guid restaurant_id { get; set; }

    public ContactDto? contact { get; set; }

    /// <example>โรบินสันลาดกระบัง</example>
    public string? name { get; set; }

    /// <example>ร้านบิ๊กบัง สาขาโรบินสันลาดกระบัง</example>
    public string? display_name { get; set; }

    /// <example>71 Lat Krabang Rd, Lat Krabang, Bangkok 10520</example>
    public string? address { get; set; }

    /// <example>10:00</example>
    public TimeOnly? opening_time { get; set; }

    /// <example>22:00</example>
    public TimeOnly? closing_time { get; set; }

    public static readonly Expression<Func<Branch, BranchResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            delete_time = model.DeleteTime,
            restaurant_id = model.RestaurantId,
            contact = model.Contact == null ? null : ContactDto.Projection.Invoke(model.Contact),
            name = model.Name,
            display_name = model.DisplayName,
            address = model.Address,
            opening_time = model.OpeningTime,
            closing_time = model.ClosingTime,
        };

    public static readonly Func<Branch, BranchResponse> Project = Projection.Compile();
}