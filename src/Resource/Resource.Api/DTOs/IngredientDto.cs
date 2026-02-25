namespace FoodSphere.Resource.Api.DTO;

public class IngredientRequest
{
    public required string name { get; set; }
    public string? description { get; set; }
    public string? image_url { get; set; }
    public string? unit { get; set; }
}

public class IngredientResponse
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    public required string name { get; set; }
    public string? description { get; set; }
    public string? image_url { get; set; }
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
            name = model.Name,
            description = model.Description,
            image_url = model.ImageUrl,
            unit = model.Unit,
            status = model.Status,
        };
    }
}