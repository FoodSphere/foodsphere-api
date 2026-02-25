namespace FoodSphere.SelfOrdering.Api.Controller;

[Route("me")]
public class MeController(
    ILogger<MeController> logger,
    BillService billService,
    ConsumerAuthService consumerAuthService
) : SelfOrderingControllerBase
{
    [HttpGet]
    public async Task<ActionResult<BillMemberResponse>> GetMemberDetails()
    {
        return BillMemberResponse.FromModel(Member);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateMemberDetails(BillMemberRequest body)
    {
        Member.Name = body.name;

        await billService.SaveChanges();

        return NoContent();
    }

    [HttpGet("consumer")]
    public async Task<ActionResult<ConsumerResponse>> GetConsumer()
    {
        var consumer = Member.Consumer;

        if (consumer is null)
        {
            return NotFound();
        }

        return ConsumerResponse.FromModel(consumer);
    }

    [HttpPost("consumer")]
    public async Task<ActionResult> SetConsumer(SetConsumerRequest body)
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