namespace FoodSphere.Pos.Api.DTO;

public record StockTransactionRequest
{
    public required short ingredient_id { get; set; }

    /// <example>10</example>
    public required decimal amount { get; set; }

    /// <example>ซื้อจากแมคโคร ประจำสัปดาห์</example>
    public string? note { get; set; }
}

public record StockTransactionResponse
{
    public required Guid id { get; set; }

    public DateTime create_time { get; set; }

    public required short ingredient_id { get; set; }

    public Guid? bill_id { get; set; }
    public short? order_id { get; set; }
    public short? order_item_id { get; set; }

    /// <example>20</example>
    public required decimal amount { get; set; }

    /// <example>100</example>
    public required decimal balance_after { get; set; }

    // /// <example>กิโล</example>
    // public string? unit { get; set; }

    public string? note { get; set; }

    public static readonly Expression<Func<StockTransaction, StockTransactionResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            ingredient_id = model.IngredientId,
            bill_id = model.BillId,
            order_id = model.OrderId,
            order_item_id = model.OrderItemId,
            amount = model.Amount,
            balance_after = model.BalanceAfter,
            note = model.Note,
        };

    public static readonly Func<StockTransaction, StockTransactionResponse> Project = Projection.Compile();
}

public class StockRequestValidator : AbstractValidator<StockTransactionRequest>
{
    public StockRequestValidator()
    {
        RuleFor(x => x.amount)
            .NotEqual(0);
    }
}