namespace FoodSphere.Common.Entity;

public interface IPortalKey
{
    public Guid Id { get; }
}

public record PortalKey(Guid Id) : IPortalKey, IEntityKey
{
    public static implicit operator PortalKey(PortalBase model) => new(model.Id);
    public static implicit operator object[](PortalKey key) => [key.Id];
}

public abstract class PortalBase : IPortalKey, IUpdatableEntityModel
{
    public required Guid Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public short UsageCount { get; protected set; } = 0;
    public TimeSpan? ValidDuration { get; set; }
    public short? MaxUsage { get; set; }

    public override string ToString() => Id.ToString();

    public bool IsExpired()
    {
        if (ValidDuration is null)
        {
            return false;
        }

        return DateTime.UtcNow > CreateTime.Add(ValidDuration.Value);
    }

    public bool ReachUsageLimit()
    {
        if (MaxUsage is null)
        {
            return false;
        }

        return UsageCount >= MaxUsage;
    }

    public bool IsUnusable => IsExpired() || ReachUsageLimit();

    public void Use()
    {
        if (ReachUsageLimit())
        {
            throw new InvalidOperationException($"portal {GetType().Name} [{Id}] has reached its usage limit.");
        }
        else
        {
            UsageCount++;
        }
    }
}

public class OrderingPortal : PortalBase
{
    public required Guid BillId { get; init; }
    public virtual Bill Bill { get; init; } = null!;
}

public class WorkerPortal : PortalBase
{
    public required Guid RestaurantId { get; init; }
    public required short BranchId { get; init; }
    public required short WorkerId { get; init; }
    public virtual WorkerUser WorkerUser { get; init; } = null!;
}