namespace FoodSphere.Resource.Api.DTO;

public class BranchRequest
{
    public ContactDto? contact { get; set; }

    public required string name { get; set; }
    public string? display_name { get; set; }
}

public class BranchResponse //: IDTO<Branch, BranchResponse>
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    public ContactDto? contact { get; set; }

    public string? name { get; set; }
    public string? display_name { get; set; }

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
        };
    }
}
