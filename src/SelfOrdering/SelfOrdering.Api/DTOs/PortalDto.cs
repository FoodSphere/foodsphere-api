namespace FoodSphere.SelfOrdering.Api.DTO;

public class PortalRequest
{
    public short? max_usage { get; set; }
    public TimeSpan? valid_duration { get; set; }
}

public class PortalResponse
{
    public Guid id { get; set; }
    public Guid bill_id { get; set; }
    // public Guid? consumer_id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public short? max_usage { get; set; }
    public short usage_count { get; set; }
    public TimeSpan? valid_duration { get; set; }

    public static PortalResponse FromModel(SelfOrderingPortal model)
    {
        return new PortalResponse
        {
            id = model.Id,
            bill_id = model.BillId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            max_usage = model.MaxUsage,
            usage_count = model.UsageCount,
            valid_duration = model.ValidDuration
        };
    }
}