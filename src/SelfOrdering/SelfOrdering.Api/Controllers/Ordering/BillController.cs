namespace FoodSphere.SelfOrdering.Api.Controller;

[Route("bill")]
public class BillController(
    ILogger<BillController> logger,
    BillServiceBase billService,
    PaymentService paymentService
) : SelfOrderingControllerBase
{
    [HttpGet]
    public async Task<ActionResult<BillResponse>> GetBill()
    {
        var response = await billService.GetBill(
            BillResponse.Projection,
            new(MemberKey.BillId));

        if (response is null)
            return NotFound();

        return response;
    }

    [HttpPut("complete")]
    public async Task<ActionResult> CloseBill()
    {
        var result = await billService.CompleteBill(
            new(MemberKey.BillId));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    [HttpGet("payments")]
    public async Task<ActionResult<ICollection<PaymentResponse>>> ListPayments(
        [FromQuery] IReadOnlyCollection<PaymentStatus> status)
    {
        Expression<Func<Payment, bool>> predicate = e =>
            e.BillId == MemberKey.BillId;

        if (status.Count > 0)
            predicate = predicate.And(e => status.Contains(e.Status));

        return await paymentService.ListPayments(
            PaymentResponse.Projection, predicate);;
    }

    [HttpGet("payments/{payment_id}")]
    public async Task<ActionResult<PaymentResponse>> GetPayment(short payment_id)
    {
        var response = await paymentService.GetPayment(
            PaymentResponse.Projection,
            new(MemberKey.BillId, payment_id));

        if (response is null)
            return NotFound();

        return response;
    }
}