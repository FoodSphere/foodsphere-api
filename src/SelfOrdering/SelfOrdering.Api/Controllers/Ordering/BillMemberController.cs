namespace FoodSphere.SelfOrdering.Api.Controller;

[Route("members")]
public class BillMemberController(
    ILogger<BillMemberController> logger,
    BillService billService,
    ConsumerAuthService consumerAuthService
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

    [HttpGet(".")]
    public async Task<ActionResult<BillMemberResponse>> GetMyMemberDetails()
    {
        return BillMemberResponse.FromModel(Member);
    }

    [HttpPut(".")]
    public async Task<ActionResult> UpdateMyMemberDetails(BillMemberRequest body)
    {
        Member.Name = body.name;

        await billService.SaveChanges();

        return NoContent();
    }

    [HttpGet("./consumer")]
    public async Task<ActionResult<ConsumerResponse>> GetMyConsumer()
    {
        var consumer = Member.Consumer;

        if (consumer is null)
        {
            return NotFound();
        }

        return ConsumerResponse.FromModel(consumer);
    }

    [HttpPost("./consumer")]
    public async Task<ActionResult> SetMyConsumer(SetConsumerRequest body)
    {
        var claims = await consumerAuthService.ValidateToken(body.token);

        if (claims is null)
        {
            return BadRequest("invalid token");
        }

        if (!claims.TryGetValue(FoodSphereClaimType.Identity.UserIdClaimType, out var consumerId))
        {
            return BadRequest("missing user id claim");
        }

        Member.ConsumerId = Guid.Parse((string)consumerId);

        await billService.SaveChanges();

        return NoContent();
    }
}