namespace FoodSphere.SelfOrdering.Api.DTO;

public record BillMemberRequest
{
    public string? name { get; set; }
}

public record BillMemberResponse
{
    public required short id { get; set; }
    public string? name { get; set; }

    public static readonly Expression<Func<BillMember, BillMemberResponse>> Projection =
        model => new()
        {
            id = model.Id,
            name = model.Name,
        };

    public static readonly Func<BillMember, BillMemberResponse> Project = Projection.Compile();
}