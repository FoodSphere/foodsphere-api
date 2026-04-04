namespace FoodSphere.Common.Contracts;

public record StockTransactionCreatedMessage
{
    public required StockTransactionKey Resource { get; init; }
    public required BranchKey Branch { get; init; }
}