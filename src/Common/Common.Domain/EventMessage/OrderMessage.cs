namespace FoodSphere.Common.Contracts;

public record OrderCreatedMessage
{
    public required OrderKey Resource { get; init; }
    public required BranchKey Branch { get; init; }
}

public record OrderStatusUpdatedMessage
{
    public required OrderKey Resource { get; init; }
    public required OrderStatus Status { get; init; }
}

public record OrderItemUpdatedMessage
{
    public required OrderItemKey Resource { get; init; }
}