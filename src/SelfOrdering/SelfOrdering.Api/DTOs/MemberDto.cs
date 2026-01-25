namespace FoodSphere.SelfOrdering.Api.DTOs;

public class BillMemberRequest
{
    public string? name { get; set; }
}

public class BillMemberResponse
{
    public short id { get; set; }
    public string? name { get; set; }

    public static BillMemberResponse FromModel(BillMember model)
    {
        return new BillMemberResponse
        {
            id = model.Id,
            name = model.Name,
        };
    }
}