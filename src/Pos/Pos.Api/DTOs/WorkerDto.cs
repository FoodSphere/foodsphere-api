namespace FoodSphere.Pos.Api.DTO;

public record WorkerRequest
{
    /// <example>ป้าเล็กสุดสวย</example>
    public required string name { get; set; }

    public ICollection<short> roles { get; set; } = [];

    /// <example>0812345678</example>
    public string? phone { get; set; }
}

public record WorkerResponse
{
    public required int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }
    public DateTime? delete_time { get; set; }

    public required Guid restaurant_id { get; set; }

    public required short branch_id { get; set; }

    /// <example>ป้าเล็กสุดสวย</example>
    public required string name { get; set; }

    public ICollection<short> roles { get; set; } = [];

    /// <example>0812345678</example>
    public string? phone { get; set; }

    // public WorkerStatus status { get; set; }

    public static readonly Expression<Func<WorkerUser, WorkerResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            delete_time = model.DeleteTime,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            name = model.Name,
            roles = model.Roles
                .Select(r => r.RoleId)
                .ToArray(),
            phone = model.Phone,
            // status = model.Status,
        };

    public static readonly Func<WorkerUser, WorkerResponse> Project = Projection.Compile();
}

public record WorkerPortalRequest
{
    public TimeSpan? valid_duration { get; set; }
}

public record WorkerPortalResponse
{
    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }

    public required Guid restaurant_id { get; set; }
    public required short branch_id { get; set; }
    public required short worker_id { get; set; }

    public required short usage_count { get; set; }
    public short? max_usage { get; set; }
    public TimeSpan? valid_duration { get; set; }

    public static readonly Expression<Func<WorkerPortal, WorkerPortalResponse>> Projection =
        model => new()
        {
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            worker_id = model.WorkerId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            max_usage = model.MaxUsage,
            usage_count = model.UsageCount,
            valid_duration = model.ValidDuration
        };

    public static readonly Func<WorkerPortal, WorkerPortalResponse> Project = Projection.Compile();
}

public record SingleWorkerResponse
{
    public required int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }
    public DateTime? delete_time { get; set; }

    public required Guid restaurant_id { get; set; }

    /// <example>ป้าเล็กสุดสวย</example>
    public required string name { get; set; }

    public ICollection<short> roles { get; set; } = [];

    /// <example>0812345678</example>
    public string? phone { get; set; }

    // public WorkerStatus status { get; set; }

    public static readonly Expression<Func<WorkerUser, SingleWorkerResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            delete_time = model.DeleteTime,
            restaurant_id = model.RestaurantId,
            name = model.Name,
            roles = model.Roles
                .Select(r => r.RoleId)
                .ToArray(),
            phone = model.Phone,
            // status = model.Status,
        };

    public static readonly Func<WorkerUser, SingleWorkerResponse> Project = Projection.Compile();
}

public record SingleWorkerPortalResponse
{
    public required Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }

    public required Guid restaurant_id { get; set; }
    public required short worker_id { get; set; }

    public required short usage_count { get; set; }
    public short? max_usage { get; set; }
    public TimeSpan? valid_duration { get; set; }

    public static readonly Expression<Func<WorkerPortal, SingleWorkerPortalResponse>> Projection =
        model => new()
        {
            id = model.Id,
            restaurant_id = model.RestaurantId,
            worker_id = model.WorkerId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            max_usage = model.MaxUsage,
            usage_count = model.UsageCount,
            valid_duration = model.ValidDuration
        };

    public static readonly Func<WorkerPortal, SingleWorkerPortalResponse> Project = Projection.Compile();
}