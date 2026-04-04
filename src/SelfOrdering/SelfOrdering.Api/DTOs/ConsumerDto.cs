namespace FoodSphere.SelfOrdering.Api.DTO;

public record ConsumerResponse
{
    public required Guid id { get; set; }
    public string? email { get; set; }
    public string? username { get; set; }
    public string? phone_number { get; set; }

    public static readonly Expression<Func<ConsumerUser, ConsumerResponse>> Projection =
        model => new()
        {
            id = model.Id,
            email = model.Email,
            username = model.UserName,
            phone_number = model.PhoneNumber,
        };

    public static readonly Func<ConsumerUser, ConsumerResponse> Project = Projection.Compile();
}

public record SetConsumerRequest
{
    public required string token { get; set; }
}