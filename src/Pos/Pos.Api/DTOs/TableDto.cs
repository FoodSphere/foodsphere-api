namespace FoodSphere.Pos.Api.DTOs;

public class TableRequest
{
    /// <example>A1</example>
    public string? name { get; set; }
}

public class TableResponse
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    public short branch_id { get; set; }

    /// <example>"A1"</example>
    public string? name { get; set; }

    public TableStatus status { get; set; }

    public static TableResponse FromModel(Table model)
    {
        return new TableResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            name = model.Name,
            status = model.Status,
        };
    }
}