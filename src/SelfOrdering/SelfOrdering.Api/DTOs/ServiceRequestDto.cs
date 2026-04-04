namespace FoodSphere.SelfOrdering.Api.DTO;

public record ServiceRequestRequest
{
    public required string reason { get; set; }
}

public record ServiceRequestResponse
{
    public required Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }

    public required string reason { get; set; }

    public ServiceRequestStatus status { get; set; }

    public static readonly Expression<Func<ServiceRequest, ServiceRequestResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            reason = model.Reason,
            status = model.Status
        };

    public static readonly Func<ServiceRequest, ServiceRequestResponse> Project = Projection.Compile();
}