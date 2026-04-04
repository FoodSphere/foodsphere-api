namespace FoodSphere.Pos.Api.DTO;

public record IngredientRequest
{
    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    public ICollection<AssignTagRequest> tags { get; set; } = [];

    /// <example>กิโล</example>
    public string? unit { get; set; }

    /// <example>เนื้อออส</example>
    public string? description { get; set; }

    public IngredientStatus status { get; set; }
}

public record IngredientResponse
{
    public required short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }
    public DateTime? delete_time { get; set; }

    public required Guid restaurant_id { get; set; }

    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    public ICollection<TagDto> tags { get; set; } = [];

    /// <example>กิโล</example>
    public string? unit { get; set; }

    /// <example>เนื้อโคขุน</example>
    public string? description { get; set; }

    public string? image_url { get; set; }

    public IngredientStatus status { get; set; }

    public static readonly Expression<Func<Ingredient, IngredientResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            delete_time = model.DeleteTime,
            restaurant_id = model.RestaurantId,
            name = model.Name,
            tags = model.Tags
                .Select(m => TagDto.IngredientTagProjection.Invoke(m))
                .ToArray(),
            unit = model.Unit,
            description = model.Description,
            image_url = model.ImageUrl,
            status = model.Status,
        };

    public static readonly Func<Ingredient, IngredientResponse> Project = Projection.Compile();
}

public record SingleIngredientRequest
{
    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    public ICollection<AssignTagRequest> tags { get; set; } = [];

    /// <example>กิโล</example>
    public string? unit { get; set; }

    public decimal stock { get; set; }

    /// <example>เนื้อออส</example>
    public string? description { get; set; }

    public IngredientStatus status { get; set; }
}

public record SingleIngredientResponse
{
    public required short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }
    public DateTime? delete_time { get; set; }

    public required Guid restaurant_id { get; set; }

    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    public ICollection<TagDto> tags { get; set; } = [];

    /// <example>20</example>
    public decimal stock { get; set; }

    /// <example>กิโล</example>
    public string? unit { get; set; }

    /// <example>เนื้อโคขุน</example>
    public string? description { get; set; }

    public string? image_url { get; set; }

    public IngredientStatus status { get; set; }

    public static readonly Expression<Func<StockTransaction, SingleIngredientResponse>> Projection =
        model => new()
        {
            id = model.Ingredient.Id,
            create_time = model.Ingredient.CreateTime,
            update_time = model.Ingredient.UpdateTime,
            delete_time = model.Ingredient.DeleteTime,
            restaurant_id = model.Ingredient.RestaurantId,
            name = model.Ingredient.Name,
            tags = model.Ingredient.Tags
                .Select(m => TagDto.IngredientTagProjection.Invoke(m))
                .ToArray(),
            stock = model.BalanceAfter,
            unit = model.Ingredient.Unit,
            description = model.Ingredient.Description,
            image_url = model.Ingredient.ImageUrl,
            status = model.Ingredient.Status,
        };

    public static readonly Func<StockTransaction, SingleIngredientResponse> Project = Projection.Compile();
}

public class SingleIngredientRequestValidator : AbstractValidator<SingleIngredientRequest>
{
    public SingleIngredientRequestValidator()
    {
        RuleFor(x => x.stock)
            .GreaterThanOrEqualTo(0);
    }
}