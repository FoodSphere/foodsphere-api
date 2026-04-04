namespace FoodSphere.Common.Constant;

public enum BillStatus
{
    Open,
    Paid, // or we should calculate by payment status?
    Completed,
    Cancelled,
}

public enum PaymentStatus
{
    Pending,
    Succeeded,
    Failed,
    Refunded,
}

public enum OrderStatus
{
    Draft,
    Pending,
    Cooking,
    Served,
    Cancelled,
}

public enum WorkerStatus
{

}

public enum TableStatus
{
    Ready,
    Disabled,
    Occupied,
    Reserved,
}

public enum MenuStatus
{
    Inactive,
    Active,
}

public enum IngredientStatus
{
    Inactive,
    Active,
}

public enum ServiceRequestStatus
{
    Pending,
    Acknowledged,
    Done,
    Cancelled,
}