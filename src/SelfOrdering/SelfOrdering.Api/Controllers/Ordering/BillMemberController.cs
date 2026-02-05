using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.SelfOrdering.Api.Controllers;

[Route("members")]
public class BillMemberController(
    ILogger<BillMemberController> logger,
    BillService billService
) : SelfOrderingControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BillMemberResponse>>> ListBillMembers()
    {
        var members = await billService.ListBillMembers(Member.BillId);

        return members
            .Select(BillMemberResponse.FromModel)
            .ToList();
    }

    [HttpGet("{member_id}")]
    public async Task<ActionResult<BillMemberResponse>> GetBillMember(short member_id)
    {
        var member = await billService.GetBillMember(Member.BillId, member_id);

        if (member is null)
        {
            return NotFound();
        }

        return BillMemberResponse.FromModel(member);
    }
}