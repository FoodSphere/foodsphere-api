namespace FoodSphere.Pos.Api.DTO;

public record ServiceRequestStatusRequest
{
    public required ServiceRequestStatus status { get; set; }
}

public record ServiceRequestResponse
{
    public required Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }

    public required Guid bill_id { get; set; }

    public required TableBriefResponse table { get; set; }

    public required string reason { get; set; }

    public ServiceRequestStatus status { get; set; }

    public static readonly Expression<Func<ServiceRequest, ServiceRequestResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            bill_id = model.BillId,
            table = TableBriefResponse.Projection.Invoke(model.Bill.Table),
            reason = model.Reason,
            status = model.Status
        };

    public static readonly Func<ServiceRequest, ServiceRequestResponse> Project = Projection.Compile();
}