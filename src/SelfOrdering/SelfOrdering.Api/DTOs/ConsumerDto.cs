namespace FoodSphere.SelfOrdering.Api.DTOs;

public class ConsumerResponse
{
    public Guid id { get; set; }
    public string? email { get; set; }
    public string? name { get; set; }
    public string? phone { get; set; }

    public static ConsumerResponse FromModel(ConsumerUser model)
    {
        return new ConsumerResponse
        {
            id = model.Id,
            email = model.Email,
            name = model.Name,
            phone = model.Phone,
        };
    }
}

public class SetConsumerRequest
{
    public required string token { get; set; }
}