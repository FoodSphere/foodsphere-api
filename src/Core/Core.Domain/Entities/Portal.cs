namespace FoodSphere.Core.Entities;

public class SelfOrderingPortal : PortalBase
{
    public Guid BillId { get; set; }
    public virtual Bill Bill { get; set; } = null!;
}

public class StaffPortal : PortalBase
{
    public Guid RestaurantId { get; set; }
    public short BranchId { get; set; }
    public short StaffId { get; set; }
    public virtual StaffUser StaffUser { get; set; } = null!;
}