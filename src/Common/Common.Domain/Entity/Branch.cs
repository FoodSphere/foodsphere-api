namespace FoodSphere.Common.Entity;

public interface IBranchKey
{
    public Guid RestaurantId { get; }
    public short Id { get; }
}

public record BranchKey(Guid RestaurantId, short Id) : IBranchKey, IEntityKey
{
    public static implicit operator BranchKey(Branch model) => new(model.RestaurantId, model.Id);
    public static implicit operator object[](BranchKey key) => [key.RestaurantId, key.Id];
}

public class Branch : IBranchKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public virtual Restaurant Restaurant { get; init; } = null!;

    public virtual ICollection<BranchStaff> BranchStaffs { get; } = [];
    public virtual ICollection<StockTransaction> IngredientStocks { get; } = [];
    public virtual ICollection<Table> Tables { get; } = [];
    public virtual ICollection<WorkerUser> Workers { get; } = [];
    public virtual ICollection<Queuing> Queuings { get; } = [];

    public Contact Contact { get; set; } = new();
    public required string Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Address { get; set; }
    public TimeOnly? OpeningTime { get; set; }
    public TimeOnly? ClosingTime { get; set; }
}