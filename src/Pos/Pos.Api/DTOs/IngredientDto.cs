namespace FoodSphere.Pos.Api.DTO;

public class IngredientRequest
{
    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    /// <example>เนื้อออส</example>
    public string? description { get; set; }

    /// <example>กิโล</example>
    public string? unit { get; set; }
}

public class IngredientResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    public TagDto[] tags { get; set; } = [];

    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    /// <example>เนื้อโคขุน</example>
    public string? description { get; set; }

    public string? image_url { get; set; }

    /// <example>กิโล</example>
    public string? unit { get; set; }

    public IngredientStatus status { get; set; }

    public static IngredientResponse FromModel(Ingredient model)
    {
        return new IngredientResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            tags = [.. model.IngredientTags.Select(TagDto.FromModel)],
            name = model.Name,
            description = model.Description,
            image_url = model.ImageUrl,
            unit = model.Unit,
            status = model.Status,
        };
    }
}

public class SingleIngredientRequest
{
    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    /// <example>เนื้อออส</example>
    public string? description { get; set; }

    /// <example>กิโล</example>
    public string? unit { get; set; }

    /// <example>20</example>
    public decimal stock { get; set;}
}

public class SingleIngredientResponse
{
    public short id { get; set; }

    public TagDto[] tags { get; set; } = [];

    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    /// <example>เนื้อออส</example>
    public string? description { get; set; }

    /// <example>กิโล</example>
    public string? unit { get; set; }

    /// <example>20</example>
    public decimal stock { get; set;}

    public static SingleIngredientResponse FromModel(Stock stock)
    {
        return new SingleIngredientResponse
        {
            id = stock.IngredientId,
            tags = [..stock.Ingredient.IngredientTags.Select(TagDto.FromModel)],
            name = stock.Ingredient.Name,
            description = stock.Ingredient.Description,
            unit = stock.Ingredient.Unit,
            stock = stock.Amount,
        };
    }
}