namespace FoodSphere.SelfOrdering.Api.DTO;

public record PortalRequest
{
    public short? max_usage { get; set; }
    public TimeSpan? valid_duration { get; set; }
}

public record PortalResponse
{
    public required Guid id { get; set; }
    public required Guid bill_id { get; set; }
    // public Guid? consumer_id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }

    public short usage_count { get; set; }
    public short? max_usage { get; set; }
    public TimeSpan? valid_duration { get; set; }

    public static readonly Expression<Func<OrderingPortal, PortalResponse>> Projection =
        model => new()
        {
            id = model.Id,
            bill_id = model.BillId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            usage_count = model.UsageCount,
            max_usage = model.MaxUsage,
            valid_duration = model.ValidDuration
        };

    public static readonly Func<OrderingPortal, PortalResponse> Project = Projection.Compile();
}

public class PortalRequestValidator : AbstractValidator<PortalRequest>
{
    public PortalRequestValidator()
    {
        RuleFor(x => x.max_usage)
            .GreaterThan((short)0)
            .When(x => x.max_usage is not null);
    }
}