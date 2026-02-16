namespace FoodSphere.Pos.Api.DTO;

public class StaffRequest
{
    /// <example>ป้าเล็กสุดสวย</example>
    public required string name { get; set; }

    public IReadOnlyCollection<short> roles { get; set; } = [];

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

    public IReadOnlyCollection<short> roles { get; set; } = [];

    /// <example>0812345678</example>
    public string? phone { get; set; }

    // public StaffStatus status { get; set; }

    public static readonly Func<StaffUser, StaffResponse> Project = Projection.Compile();

    public static Expression<Func<StaffUser, StaffResponse>> Projection =>
        model => new StaffResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            name = model.Name,
            roles = model.Roles
                .Select(r => r.RoleId)
                .ToArray(),
            phone = model.Phone,
            // status = model.Status,
        };
}

public class StaffPortalRequest
{
    public TimeSpan? valid_duration { get; set; }
}

public class StaffPortalResponse
{
    public Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }
    public short staff_id { get; set; }

    public short? max_usage { get; set; }
    public short usage_count { get; set; }
    public TimeSpan? valid_duration { get; set; }

    public static readonly Func<StaffPortal, StaffPortalResponse> Project = Projection.Compile();

    public static Expression<Func<StaffPortal, StaffPortalResponse>> Projection =>
        model => new StaffPortalResponse
        {
            id = model.Id,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            staff_id = model.StaffId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            max_usage = model.MaxUsage,
            usage_count = model.UsageCount,
            valid_duration = model.ValidDuration
        };
}

public class SingleStaffResponse
{
    public int id { get; set; }

    public Guid restaurant_id { get; set; }

    /// <example>ป้าเล็กสุดสวย</example>
    public required string name { get; set; }

    public IReadOnlyCollection<short> roles { get; set; } = [];

    /// <example>0812345678</example>
    public string? phone { get; set; }

    // public StaffStatus status { get; set; }

    public static readonly Func<StaffUser, SingleStaffResponse> Project = Projection.Compile();

    public static Expression<Func<StaffUser, SingleStaffResponse>> Projection =>
        model => new SingleStaffResponse
        {
            id = model.Id,
            restaurant_id = model.RestaurantId,
            name = model.Name,
            roles = model.Roles
                .Select(r => r.RoleId)
                .ToArray(),
            phone = model.Phone,
            // status = model.Status,
        };
}

public class SingleStaffPortalResponse
{
    public Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }
    public short staff_id { get; set; }

    public short? max_usage { get; set; }
    public short usage_count { get; set; }
    public TimeSpan? valid_duration { get; set; }

    public static readonly Func<StaffPortal, SingleStaffPortalResponse> Project = Projection.Compile();

    public static Expression<Func<StaffPortal, SingleStaffPortalResponse>> Projection =>
        model => new SingleStaffPortalResponse
        {
            id = model.Id,
            restaurant_id = model.RestaurantId,
            staff_id = model.StaffId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            max_usage = model.MaxUsage,
            usage_count = model.UsageCount,
            valid_duration = model.ValidDuration
        };
}