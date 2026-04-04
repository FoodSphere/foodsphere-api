namespace FoodSphere.Pos.Api.DTO;

public record StaffRequest
{
    public required string master_id { get; set; }
    public required string display_name { get; set; }

    public ICollection<short> roles { get; set; } = [];
}

public record UpdateStaffRequest
{
    public ICollection<short> roles { get; set; } = [];
}

public record BranchStaffResponse
{
    public required string master_id { get; set; }
    public required Guid restaurant_id { get; set; }
    public required short branch_id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }
    public DateTime? delete_time { get; set; }

    public ICollection<int> permissions { get; set; } = [];

    public static readonly Expression<Func<BranchStaff, BranchStaffResponse>> Projection =
        model => new()
        {
            master_id = model.MasterId,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            delete_time = model.DeleteTime,
            permissions = model.Roles
                .SelectMany(r => r.Role.Permissions)
                .Select(rp => rp.Permission)
                .Select(p => p.Id)
                .Distinct()
                .ToArray(),
        };

    public static readonly Func<BranchStaff, BranchStaffResponse> Project = Projection.Compile();
}

public record RestaurantStaffResponse
{
    public required string master_id { get; set; }
    public required Guid restaurant_id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }
    public DateTime? delete_time { get; set; }

    public ICollection<int> permissions { get; set; } = [];

    public static readonly Expression<Func<RestaurantStaff, RestaurantStaffResponse>> Projection =
        model => new()
        {
            master_id = model.MasterId,
            restaurant_id = model.RestaurantId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            delete_time = model.DeleteTime,
            permissions = model.Roles
                .SelectMany(r => r.Role.Permissions)
                .Select(rp => rp.Permission)
                .Select(p => p.Id)
                .Distinct()
                .ToArray(),
        };

    public static readonly Func<RestaurantStaff, RestaurantStaffResponse> Project = Projection.Compile();
}