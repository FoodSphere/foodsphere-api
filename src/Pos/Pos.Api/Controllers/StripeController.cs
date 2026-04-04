namespace FoodSphere.Pos.Api.Controller;

[Route("stripe")]
public class StripeController(
    ILogger<PaymentController> logger,
    PaymentService paymentService
) : PosControllerBase
{
    /// <summary>
    /// webhook for stripe
    /// </summary>
    [AllowAnonymous]
    [HttpPost("webhook/payment")]
    public async Task<ActionResult> PaymentWebhook(StripeWebhookRequest body)
    {
        var BillId = Guid.Parse(body.data.@object.metadata.bill_id);

        var result = await paymentService.CreateStripePayment(
            e => true, new(
                BillKey: new(BillId),
                SessionId: body.data.@object.id,
                Amount: body.data.@object.amount_total / 100m));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (key, response) = result.Value;

        await paymentService.HandleStripePaymentWebhook(key);

        return Ok();
    }
}