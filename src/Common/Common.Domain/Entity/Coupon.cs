namespace FoodSphere.Common.Entity;

public interface ICouponKey
{
    public Guid RestaurantId { get; }
    public Guid Id { get; }
}

public record CouponKey(Guid RestaurantId, Guid Id) : ICouponKey, IEntityKey
{
    public static implicit operator CouponKey(Coupon model) => new(model.RestaurantId, model.Id);
    public static implicit operator object[](CouponKey key) => [key.RestaurantId, key.Id];
}

public class Coupon : ICouponKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required Guid Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public required string Code { get; set; }
    public decimal? PercentageDiscount { get; set; }
    public int? FixedDiscount { get; set; }
    public int? MaxUsage { get; set; }

    public required DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
}