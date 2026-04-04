namespace FoodSphere.Common.Contracts;

public record ServiceRequestCreatedMessage
{
    public required ServiceRequestKey Resource { get; init; }
    public required TableKey Table { get; init; }
}

public record ServiceRequestStatusUpdatedMessage
{
    public required ServiceRequestKey Resource { get; init; }
    public required ServiceRequestStatus Status { get; init; }
}