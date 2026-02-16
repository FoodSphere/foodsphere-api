namespace FoodSphere.SelfOrdering.Api.Controller;

public class MemberController(
    ILogger<MemberController> logger,
    BillService billService,
    ConsumerAuthService consumerAuthService
) : SelfOrderingControllerBase
{
    /// <summary>
    /// list bill's members
    /// </summary>
    [HttpGet("members")]
    public async Task<ActionResult<ICollection<BillMemberResponse>>> ListMembers()
    {
        var responses = await billService.QueryMembers(Member.BillId)
            .Select(BillMemberResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// get member
    /// </summary>
    [HttpGet("members/{member_id}")]
    public async Task<ActionResult<BillMemberResponse>> GetMember(short member_id)
    {
        var response = await billService.GetMember(Member.BillId, member_id, BillMemberResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// get current member
    /// </summary>
    [HttpGet("current/member")]
    public async Task<ActionResult<BillMemberResponse>> GetCurrentMember()
    {
        var response = await billService.GetMember(Member.BillId, Member.Id, BillMemberResponse.Projection);

        if (response is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return response;
    }

    /// <summary>
    /// update current member
    /// </summary>
    [HttpPut("current/member")]
    public async Task<ActionResult> UpdateCurrentMember(BillMemberRequest body)
    {
        Member.Name = body.name;

        await billService.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// get current consumer account
    /// </summary>
    [HttpGet("current/member/consumer")]
    public async Task<ActionResult<ConsumerResponse>> GetCurrentConsumer()
    {
        var response = await billService.QueryMembers()
            .Where(e =>
                e.BillId == Member.BillId &&
                e.Id == Member.Id &&
                e.ConsumerId != null)
            .Select(e => ConsumerResponse.Projection.Invoke(e.Consumer!))
            .SingleOrDefaultAsync();

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// upsert member's consumer account to by token
    /// </summary>
    [HttpPut("current/member/consumer")]
    public async Task<ActionResult> SetCurrentConsumer(SetConsumerRequest body)
    {
        var claims = await consumerAuthService.ValidateToken(body.token);

        if (claims is null ||
            !claims.TryGetValue(FoodSphereClaimType.Identity.UserIdClaimType, out var consumerIdClaim) ||
            !Guid.TryParse((string)consumerIdClaim, out var consumerId))
        {
            return BadRequest("invalid claim");
        }

        Member.ConsumerId = consumerId;

        await billService.SaveChanges();

        return NoContent();
    }
}