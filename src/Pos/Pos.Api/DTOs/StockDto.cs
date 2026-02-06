namespace FoodSphere.Pos.Api.DTO;

public class StockDto
{
    public short ingredient_id { get; set; }

    /// <example>20</example>
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