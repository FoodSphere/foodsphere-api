namespace FoodSphere.Common.Entity;

public interface IConsumerUserKey
{
    public Guid Id { get; }
}

public record ConsumerUserKey(Guid Id) : IConsumerUserKey, IEntityKey
{
    public static readonly Expression<Func<ConsumerUser, object>> KeyExpression =
        model => new
        {
            model.Id,
        };

    public static implicit operator ConsumerUserKey(ConsumerUser model) => new(model.Id);
    public static implicit operator object[](ConsumerUserKey key) => [key.Id];
}

public class ConsumerUser : IConsumerUserKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required Guid Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public virtual ICollection<Bill> Bills { get; } = [];

    public required string Email { get; set; }
    public required string UserName { get; set; }
    public required string PasswordHash { get; set; }
    public string? PhoneNumber { get; set; }
    public bool TwoFactorEnabled { get; set; } = false;

    public string GetSecurityStamp()
    {
        throw new NotImplementedException();
    }
}