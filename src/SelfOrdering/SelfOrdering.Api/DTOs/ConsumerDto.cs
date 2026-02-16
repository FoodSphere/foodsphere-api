namespace FoodSphere.SelfOrdering.Api.DTO;

public class ConsumerResponse
{
    public Guid id { get; set; }
    public string? email { get; set; }
    public string? name { get; set; }
    public string? phone { get; set; }

    public static readonly Func<ConsumerUser, ConsumerResponse> Project = Projection.Compile();

    public static Expression<Func<ConsumerUser, ConsumerResponse>> Projection =>
        model => new ConsumerResponse
        {
            id = model.Id,
            email = model.Email,
            name = model.Name,
            phone = model.Phone,
        };
}

public class SetConsumerRequest
{
    public required string token { get; set; }
}