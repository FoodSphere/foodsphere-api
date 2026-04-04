namespace FoodSphere.Common.Entity;

public interface IStockTransactionKey
{
    public Guid RestaurantId { get; }
    public short BranchId { get; }
    public Guid Id { get; }
}

public record StockTransactionKey(Guid RestaurantId, short BranchId, Guid Id) : IStockTransactionKey, IEntityKey
{
    public static implicit operator StockTransactionKey(StockTransaction model) => new(model.RestaurantId, model.BranchId, model.Id);
    public static implicit operator object[](StockTransactionKey key) => [key.RestaurantId, key.BranchId, key.Id];
}

public class StockTransaction : IStockTransactionKey, IEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short BranchId { get; init; }
    public required Guid Id { get; init; }

    public DateTime CreateTime { get; set; }

    public required short IngredientId { get; init; }
    public virtual Ingredient Ingredient { get; init; } = null!;

    public Guid? BillId { get; set; }
    public short? OrderId { get; set; }
    public short? OrderItemId { get; set; }
    public virtual OrderItem? OrderItem { get; set; }

    public required decimal Amount { get; set; }
    public required decimal BalanceAfter { get; set; }
    public string? Note { get; set; }
}