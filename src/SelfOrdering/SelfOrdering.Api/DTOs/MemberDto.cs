namespace FoodSphere.SelfOrdering.Api.DTO;

public class BillMemberRequest
{
    public string? name { get; set; }
}

public class BillMemberResponse
{
    public short id { get; set; }
    public string? name { get; set; }

    public static readonly Func<BillMember, BillMemberResponse> Project = Projection.Compile();

    public static Expression<Func<BillMember, BillMemberResponse>> Projection =>
        model => new BillMemberResponse
        {
            id = model.Id,
            name = model.Name,
        };
}