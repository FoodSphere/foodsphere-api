namespace FoodSphere.Pos.Api.DTO;

public record RevenueResponse
{
    public required decimal revenue { get; set; }
}

public record MenuSoldResponse
{
    public required int menu_sold { get; set; }
}

public record BillCountResponse
{
    public required int bill_count { get; set; }
}