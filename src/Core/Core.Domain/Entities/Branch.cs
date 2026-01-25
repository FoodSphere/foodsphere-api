namespace FoodSphere.Core.Entities;

public class Branch : EntityBase<short>
{
    public Guid RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;

    public Guid? ContactId { get; set; }
    public virtual Contact? Contact { get; set; } = null!;

    public virtual List<Manager> BranchManagers { get; } = [];
    public virtual List<Stock> IngredientStocks { get; } = [];
    public virtual List<Table> Tables { get; } = [];
    public virtual List<StaffUser> Staffs { get; } = [];
    public virtual List<Queuing> Queuings { get; } = [];

    public required string Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Address { get; set; }
    public TimeOnly? OpeningTime { get; set; }
    public TimeOnly? ClosingTime { get; set; }
}

public class Manager : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }
    public short BranchId { get; set; }
    public virtual Branch Branch { get; set; } = null!;

    public string MasterId { get; set; } = null!;
    public virtual MasterUser Master { get; set; } = null!;

    public virtual List<ManagerRole> Roles { get; } = [];
}

public class ManagerRole : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }
    public short BranchId { get; set; }

    public string ManagerId { get; set; } = null!;
    public virtual Manager ManagerBranch { get; set; } = null!;

    public short RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
}