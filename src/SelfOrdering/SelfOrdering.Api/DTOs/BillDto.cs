namespace FoodSphere.SelfOrdering.Api.DTO;

public record BillResponse
{
    public required Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }

    public TableBriefResponse table { get; set; } = null!;

    public Guid? consumer_id { get; set; }

    /// <example>2</example>
    public short? pax { get; set; }

    public required BillStatus status { get; set; }

    public static readonly Expression<Func<Bill, BillResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            table = TableBriefResponse.Projection.Invoke(model.Table),
            consumer_id = model.ConsumerId,
            pax = model.Pax,
            status = model.Status,
        };

    public static readonly Func<Bill, BillResponse> Project = Projection.Compile();
}