namespace FoodSphere.Data.Models
{
    public interface IModel<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
    }

    public interface ITrackableModel
    {
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public class TrackableModel : ITrackableModel
    {
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public DateTime UpdateTime { get; set; }
    }

    public class BaseModel<TKey> : IModel<TKey>, ITrackableModel
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; } = default!;

        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public DateTime UpdateTime { get; set; }
    }

    public class BaseModel : BaseModel<Guid>
    {
        public BaseModel()
        {
            Id = Guid.NewGuid();
        }
    }

    public class BasePortal : BaseModel
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
}

namespace FoodSphere.Data.DTOs
{
    // public interface IDTO<T, U>
    //     where T : Models.IModel
    //     where U : IDTO<T, U>
    // {
    //     public static abstract U FromModel(T model);
    // }
}