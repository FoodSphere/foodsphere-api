namespace FoodSphere.Resource.Api.DTOs;

public class StockDto
{
    public short ingredient_id { get; set; }
    public decimal amount { get; set; }

    public static StockDto FromModel(Stock model)
    {
        return new StockDto
        {
            ingredient_id = model.IngredientId,
            amount = model.Amount,
        };
    }
}