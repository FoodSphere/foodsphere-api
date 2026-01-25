namespace FoodSphere.Core.Enums;

public enum BillStatus
{
    Pending,
    Processing,
    Completed,
    Canceled,
}

public enum OrderStatus
{
    Pending,
    Processing,
    Completed,
    Canceled,
}

public enum StaffStatus
{
    Pending,
    Cooking,
    Done,
    Cancelled,
}

public enum TableStatus
{
    Open,
    Closed,
    Occupied,
    Reserved,
    Archived
}

public enum MenuStatus
{
    Active,
    Inactive,
    Archived
}

public enum IngredientStatus
{
    Active,
    Inactive,
    Archived
}

public enum StockStatus
{
    Active,
    Inactive,
    OutOfStock
}