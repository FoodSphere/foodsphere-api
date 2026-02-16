namespace FoodSphere.Pos.Api.DTO;

public class IngredientRequest
{
    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    public IReadOnlyCollection<AssignTagRequest> tags { get; set; } = [];

    /// <example>กิโล</example>
    public string? unit { get; set; }

    /// <example>เนื้อออส</example>
    public string? description { get; set; }
}

public class IngredientResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    public IReadOnlyCollection<TagDto> tags { get; set; } = [];

    /// <example>กิโล</example>
    public string? unit { get; set; }

    /// <example>เนื้อโคขุน</example>
    public string? description { get; set; }

    public string? image_url { get; set; }

    public IngredientStatus status { get; set; }

    public static readonly Func<Ingredient, IngredientResponse> Project = Projection.Compile();

    public static Expression<Func<Ingredient, IngredientResponse>> Projection =>
        model => new IngredientResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            name = model.Name,
            tags = model.IngredientTags
                .Select(m => TagDto.IngredientTagProjection.Invoke(m))
                .ToArray(),
            unit = model.Unit,
            description = model.Description,
            image_url = model.ImageUrl,
            status = model.Status,
        };
}

public class SingleIngredientRequest
{
    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    /// <example>20</example>
    public decimal stock { get; set;}

    public IReadOnlyCollection<AssignTagRequest> tags { get; set; } = [];

    /// <example>กิโล</example>
    public string? unit { get; set; }

    /// <example>เนื้อออส</example>
    public string? description { get; set; }
}

public class SingleIngredientResponse
{
    public short id { get; set; }

    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    public IReadOnlyCollection<TagDto> tags { get; set; } = [];

    /// <example>20</example>
    public decimal stock { get; set;}

    /// <example>กิโล</example>
    public string? unit { get; set; }

    /// <example>เนื้อออส</example>
    public string? description { get; set; }

    public string? image_url { get; set; }

    public IngredientStatus status { get; set; }

    public static readonly Func<Stock, SingleIngredientResponse> Project = Projection.Compile();

    public static Expression<Func<Stock, SingleIngredientResponse>> Projection =>
        model => new SingleIngredientResponse
        {
            id = model.IngredientId,
            name = model.Ingredient.Name,
            tags = model.Ingredient.IngredientTags
                .Select(m => TagDto.IngredientTagProjection.Invoke(m))
                .ToArray(),
            stock = model.Amount,
            unit = model.Ingredient.Unit,
            description = model.Ingredient.Description,
            image_url = model.Ingredient.ImageUrl,
            status = model.Ingredient.Status,
        };
}