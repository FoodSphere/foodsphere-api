namespace FoodSphere.Common.Contracts;

public record PaymentCreatedMessage
{
    public required PaymentKey Resource { get; init; }
    public required BranchKey Branch { get; init; }
}

public record PaymentStatusUpdatedMessage
{
    public required PaymentKey Resource { get; init; }
    public required PaymentStatus Status { get; init; }
}