namespace FoodSphere.Pos.Api.DTO;

public class TagRequest
{
    /// <example>meat</example>
    public required string name { get; set; }
}

public class TagResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    /// <example>meat</example>
    public required string name { get; set; }

    public static TagResponse FromModel(Tag model)
    {
        return new TagResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            name = model.Name,
        };
    }
}

public class TagDto
{
    public short tag_id { get; set; }

    /// <example>meat</example>
    public string name { get; set; } = string.Empty;

    public static TagDto FromModel(MenuTag model)
    {
        return new TagDto
        {
            tag_id = model.TagId,
            name = model.Tag.Name,
        };
    }

    public static TagDto FromModel(IngredientTag model)
    {
        return new TagDto
        {
            tag_id = model.TagId,
            name = model.Tag.Name,
        };
    }
}

public class AssignTagRequest
{
    public short tag_id { get; set; }
}