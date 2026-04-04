namespace FoodSphere.Common.Contracts;

public record TableCreatedMessage
{
    public required TableKey Resource { get; init; }
}

public record TableStatusUpdatedMessage
{
    public required TableKey Resource { get; init; }
    public required TableStatus Status { get; init; }
}