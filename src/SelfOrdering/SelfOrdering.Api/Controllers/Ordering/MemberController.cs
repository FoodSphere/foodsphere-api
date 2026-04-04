namespace FoodSphere.SelfOrdering.Api.Controller;

public class MemberController(
    ILogger<MemberController> logger,
    MemberServiceBase memberService,
    ConsumerAuthService consumerAuthService,
    ConsumerServiceBase consumerService
) : SelfOrderingControllerBase
{
    /// <summary>
    /// list bill's members
    /// </summary>
    [HttpGet("members")]
    public async Task<ActionResult<ICollection<BillMemberResponse>>> ListMembers()
    {
        Expression<Func<BillMember, bool>> predicate = e =>
            e.BillId == MemberKey.BillId;

        return await memberService.ListMembers(
            BillMemberResponse.Projection, predicate);
    }

    /// <summary>
    /// get member
    /// </summary>
    [HttpGet("members/{member_id}")]
    public async Task<ActionResult<BillMemberResponse>> GetMember(
        short member_id)
    {
        var response = await memberService.GetMember(
            BillMemberResponse.Projection,
            new(MemberKey.BillId, member_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// get current member
    /// </summary>
    [HttpGet("current/member")]
    public async Task<ActionResult<BillMemberResponse>> GetCurrentMember()
    {
        var response = await memberService.GetMember(
            BillMemberResponse.Projection, MemberKey);

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// update current member
    /// </summary>
    [HttpPut("current/member")]
    public async Task<ActionResult> UpdateCurrentMember(BillMemberRequest body)
    {
        var result = await memberService.UpdateMember(
            MemberKey, new(body.name));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// get current consumer account
    /// </summary>
    [HttpGet("current/member/consumer")]
    public async Task<ActionResult<ConsumerResponse>> GetCurrentConsumer()
    {
        var member = await memberService.GetMember(
            e => new { e.ConsumerId }, MemberKey);

        if (member?.ConsumerId is not Guid consumerId)
            return NotFound();

        var response = await consumerService.GetConsumer(
            ConsumerResponse.Projection,
            new(consumerId));

        if (response is null)
            return NotFound();

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
            !claims.TryGetValue(FoodSphereClaimType.Identity.UserIdClaimType, out var idClaim) ||
            !Guid.TryParse((string)idClaim, out var consumerId))
        {
            return BadRequest("invalid claim");
        }

        await memberService.SetConsumer(
            MemberKey, new(consumerId));

        return NoContent();
    }
}