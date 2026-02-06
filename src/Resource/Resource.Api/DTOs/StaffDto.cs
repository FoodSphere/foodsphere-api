namespace FoodSphere.Resource.Api.DTO;

public class StaffRequest
{
    public required string name { get; set; }
    public List<short> roles { get; set; } = [];
    public string? phone { get; set; }
}

public class StaffResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }

    public required string Name { get; set; }
    public string? Phone { get; set; }

    // public StaffStatus status { get; set; }

    public static StaffResponse FromModel(StaffUser model)
    {
        return new StaffResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            Name = model.Name,
            Phone = model.Phone,
            // status = model.Status,
        };
    }
}