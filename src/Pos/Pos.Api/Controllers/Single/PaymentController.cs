namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/bills/{bill_id}")]
public class PaymentController(
    ILogger<PaymentController> logger,
    PaymentService paymentService
) : PosControllerBase
{
    /// <summary>
    /// list payments in this bill
    /// </summary>
    [HttpGet("payments")]
    public async Task<ActionResult<ICollection<PaymentResponse>>> ListPayments(
        Guid restaurant_id, Guid bill_id,
        [FromQuery] IReadOnlyCollection<PaymentStatus> status)
    {
        Expression<Func<Payment, bool>> predicate = e =>
            e.Bill.RestaurantId == restaurant_id &&
            e.Bill.BranchId == 1 &&
            e.BillId == bill_id;

        if (status.Count != 0)
            predicate = predicate.And(e => status.Contains(e.Status));

        return await paymentService.ListPayments(
            PaymentResponse.Projection, predicate);
    }

    /// <summary>
    /// create cash payment
    /// </summary>
    [HttpPost("cash-payments")]
    public async Task<ActionResult<PaymentResponse>> CreateCashPayment(
        Guid restaurant_id, Guid bill_id)
    {
        var result = await paymentService.CreateCashPayment(
            PaymentResponse.Projection, new(
                BillKey: new(bill_id)));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (key, response) = result.Value;

        return CreatedAtAction(
            nameof(GetPayment),
            new { restaurant_id, bill_id, payment_id = key.Id },
            response);
    }

    /// <summary>
    /// get payment
    /// </summary>
    [HttpGet("payments/{payment_id}")]
    public async Task<ActionResult<PaymentResponse>> GetPayment(
        Guid restaurant_id, Guid bill_id, short payment_id)
    {
        var response = await paymentService.GetPayment(
            PaymentResponse.Projection,
            new(bill_id, payment_id));

        if (response is null)
            return NotFound();

        return response;
    }
}