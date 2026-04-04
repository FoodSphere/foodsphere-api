namespace FoodSphere.Pos.Api.DTO;

public record BillRequest
{
    public required short table_id { get; set; }

    public Guid? consumer_id { get; set; }

    /// <example>2</example>
    public short? pax { get; set; }
}

public record BillResponse
{
    public required Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }
    public DateTime? delete_time { get; set; }

    public required Guid restaurant_id { get; set; }
    public required short branch_id { get; set; }
    public required short table_id { get; set; }
    public Guid? consumer_id { get; set; }

    public ICollection<OrderResponse> orders { get; set; } = [];

    /// <example>2</example>
    public short? pax { get; set; }

    public BillStatus status { get; set; }

    public static readonly Expression<Func<Bill, BillResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            consumer_id = model.ConsumerId,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            table_id = model.TableId,
            orders = model.Orders
                .Select(e => OrderResponse.Projection.Invoke(e))
                .ToArray(),
            pax = model.Pax,
            status = model.Status,
        };

    public static readonly Func<Bill, BillResponse> Project = Projection.Compile();
}

public record OrderingPortalRequest
{
    /// <example>2</example>
    public short? max_usage { get; set; }

    public TimeSpan? valid_duration { get; set; }
}

public record OrderingPortalResponse
{
    public required Guid id { get; set; }
    public required Guid bill_id { get; set; }
    // public Guid? consumer_id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }

    /// <example>0</example>
    public required short usage_count { get; set; }

    /// <example>2</example>
    public short? max_usage { get; set; }

    public TimeSpan? valid_duration { get; set; }

    public static readonly Expression<Func<OrderingPortal, OrderingPortalResponse>> Projection =
        model => new()
        {
            id = model.Id,
            bill_id = model.BillId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            max_usage = model.MaxUsage,
            usage_count = model.UsageCount,
            valid_duration = model.ValidDuration,
        };

    public static readonly Func<OrderingPortal, OrderingPortalResponse> Project = Projection.Compile();
}

public class BillRequestValidator : AbstractValidator<BillRequest>
{
    public BillRequestValidator()
    {
        RuleFor(x => x.pax)
            .GreaterThan((short)0)
            .When(x => x.pax is not null);
    }
}

public class OrderingPortalRequestValidator : AbstractValidator<OrderingPortalRequest>
{
    public OrderingPortalRequestValidator()
    {
        RuleFor(x => x.max_usage)
            .GreaterThan((short)0)
            .When(x => x.max_usage is not null);
    }
}