namespace FoodSphere.Pos.Api.DTO;

public class BranchRequest
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

public class BranchResponse //: IDTO<Branch, BranchResponse>
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

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

    public static BranchResponse FromModel(Branch model)
    {
        return new BranchResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            contact = ContactDto.FromModel(model.Contact),
            name = model.Name,
            display_name = model.DisplayName,
            address = model.Address,
            opening_time = model.OpeningTime,
            closing_time = model.ClosingTime,
        };
    }
}