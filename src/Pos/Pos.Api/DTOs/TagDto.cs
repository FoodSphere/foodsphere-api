using System.ComponentModel.DataAnnotations;

namespace FoodSphere.Pos.Api.DTO;

public record TagRequest
{
    /// <example>meat</example>
    public required string name { get; set; }

    /// <example>ingredient</example>
    [AllowedValues(TagType.Menu, TagType.Ingredient, TagType.Default)]
    public string type { get; set; } = TagType.Default;
}

public record TagResponse
{
    public required short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }

    public required Guid restaurant_id { get; set; }

    /// <example>meat</example>
    public required string name { get; set; }

    public required string type { get; set; }

    public static readonly Expression<Func<Tag, TagResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            name = model.Name,
            type = model.Type,
        };

    public static readonly Func<Tag, TagResponse> Project = Projection.Compile();
}

public record TagDto
{
    public required short tag_id { get; set; }

    /// <example>meat</example>
    public required string name { get; set; }

    public static readonly Expression<Func<TagMenu, TagDto>> MenuTagProjection =
        model => new()
        {
            tag_id = model.TagId,
            name = model.Tag.Name,
        };

    public static readonly Func<TagMenu, TagDto> MenuTagProject = MenuTagProjection.Compile();

    public static readonly Expression<Func<TagIngredient, TagDto>> IngredientTagProjection =
        model => new()
        {
            tag_id = model.TagId,
            name = model.Tag.Name,
        };

    public static readonly Func<TagIngredient, TagDto> IngredientTagProject = IngredientTagProjection.Compile();
}

public record AssignTagRequest
{
    public required short tag_id { get; set; }
}