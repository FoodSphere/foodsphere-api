namespace FoodSphere.Core.Entities;

public interface IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
}

public interface ITrackableEntity
{
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}

public abstract class TrackableEntityBase : ITrackableEntity
{
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdateTime { get; set; }
}

public abstract class EntityBase<TKey> : IEntity<TKey>, ITrackableEntity
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;

    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdateTime { get; set; }
}

public abstract class EntityBase : EntityBase<Guid>
{
    public EntityBase()
    {
        Id = Guid.NewGuid();
    }
}

public abstract class PortalBase : EntityBase
{
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

    bool ReachUsageLimit()
    {
        if (MaxUsage is null)
        {
            return false;
        }

        return UsageCount >= MaxUsage;
    }

    public bool IsValid()
    {
        return !IsExpired() && !ReachUsageLimit();
    }

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