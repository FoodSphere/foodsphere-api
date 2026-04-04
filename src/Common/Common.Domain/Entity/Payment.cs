namespace FoodSphere.Common.Entity;

public interface IPaymentKey
{
    public Guid BillId { get; }
    public short Id { get; }
}

public record PaymentKey(Guid BillId, short Id) : IPaymentKey, IEntityKey
{
    public static implicit operator PaymentKey(Payment model) => new(model.BillId, model.Id);
    public static implicit operator object[](PaymentKey key) => [key.BillId, key.Id];
}

public abstract class Payment : IPaymentKey, IUpdatableEntityModel
{
    public required Guid BillId { get; init; }
    public required short Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual Bill Bill { get; init; } = null!;

    public string PaymentMethod { get; protected init; } = null!;
    public required decimal Amount { get; init; }

    public PaymentStatus Status { get; set; }
}

public class CashPayment : Payment
{
    public static readonly string CODE = "cash";

    public CashPayment()
    {
        PaymentMethod = CODE;
    }
}

public class StripePayment : Payment
{
    public static readonly string CODE = "stripe";

    public StripePayment()
    {
        PaymentMethod = CODE;
    }

    public required string SessionId { get; init; }
}