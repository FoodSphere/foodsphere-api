namespace FoodSphere.Pos.Api.DTO;

public class StaffRequest
{
    /// <example>ป้าเล็กสุดสวย</example>
    public required string name { get; set; }

    public short[] roles { get; set; } = [];

    /// <example>0812345678</example>
    public string? phone { get; set; }
}

public class StaffResponse
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    public short branch_id { get; set; }

    /// <example>ป้าเล็กสุดสวย</example>
    public required string name { get; set; }

    public short[] roles { get; set; } = [];

    /// <example>0812345678</example>
    public string? phone { get; set; }

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
            name = model.Name,
            roles = [..model.Roles.Select(r => r.RoleId)],
            phone = model.Phone,
            // status = model.Status,
        };
    }
}

public class StaffPortalResponse
{
    public Guid id { get; set; }
    public Guid restaurant_id { get; set; }

    public short branch_id { get; set; }
    public short staff_id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    // public short? max_usage { get; set; }
    // public short usage_count { get; set; }

    public TimeSpan? valid_duration { get; set; }

    public static StaffPortalResponse FromModel(StaffPortal model)
    {
        return new StaffPortalResponse
        {
            id = model.Id,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            staff_id = model.StaffId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            // max_usage = model.MaxUsage,
            // usage_count = model.UsageCount,
            valid_duration = model.ValidDuration
        };
    }
}