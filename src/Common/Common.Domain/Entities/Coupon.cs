namespace FoodSphere.Common.Entities;

public class Coupon : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }
    public required string Code { get; set; }

    public decimal PercentageDiscount { get; set; }
    public int FixedDiscount { get; set; }
    public int MaxUsage { get; set; }

    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
}