namespace FoodSphere.Pos.Api.DTOs;

public class BranchRequest
{
    public ContactDto? contact { get; set; }

    public required string name { get; set; }
    public string? display_name { get; set; }
    public string? address { get; set; }
    public TimeOnly? opening_time { get; set; }
    public TimeOnly? closing_time { get; set; }
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
    public string? address { get; set; }
    public TimeOnly? opening_time { get; set; }
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

public class ManagerRequest
{
    public Guid restaurant_id;
    public short branch_id;
    public required string master_id;

}

public class ManagerResponse
{
    public Guid restaurant_id;

    public DateTime create_time;
    public DateTime update_time;

    public short branch_id;
    public required string master_id;

    public static ManagerResponse FromModel(Manager model)
    {
        return new ManagerResponse
        {
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            master_id = model.MasterId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
        };
    }
}