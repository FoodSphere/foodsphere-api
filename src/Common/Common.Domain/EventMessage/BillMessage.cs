namespace FoodSphere.Common.Contracts;

public record BillCreatedMessage
{
    public required BillKey Resource { get; init; }
    public required BranchKey Branch { get; init; }
}

public record BillStatusUpdatedMessage
{
    public required BillKey Resource { get; init; }
    public required BranchKey Branch { get; init; }
    public required BillStatus Status { get; init; }
}