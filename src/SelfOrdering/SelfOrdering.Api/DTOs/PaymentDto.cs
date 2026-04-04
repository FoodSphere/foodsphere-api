namespace FoodSphere.SelfOrdering.Api.DTO;

public record PaymentRequest
{
}

public record PaymentResponse
{
    public required short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }

    public required string payment_method { get; set; }
    public required decimal amount { get; set; }

    public PaymentStatus status { get; set; }

    public static readonly Expression<Func<Payment, PaymentResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            payment_method = model.PaymentMethod,
            amount = model.Amount,
            // details =
            status = model.Status,
        };

    public static readonly Func<Payment, PaymentResponse> Project = Projection.Compile();
}