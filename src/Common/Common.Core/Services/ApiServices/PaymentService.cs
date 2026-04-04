namespace FoodSphere.Common.Service;

public class PaymentService(
    PersistenceService persistenceService,
    IPublishEndpoint publishEndpoint,
    PaymentRepository paymentRepository,
    OrderingCalculator calculatorService
) : ServiceBase
{
    public async Task<TDto[]> ListPayments<TDto, TPayment>(
        Expression<Func<TPayment, TDto>> projection,
        Expression<Func<TPayment, bool>> predicate,
        CancellationToken ct = default) where TPayment : Payment
    {
        return await paymentRepository.QueryPayments()
            .OfType<TPayment>()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(PaymentKey, TDto)>> CreateCashPayment<TDto>(
        Expression<Func<Payment, TDto>> projection,
        CashPaymentCreateCommand command,
        CancellationToken ct = default)
    {
        var totalAmount = await calculatorService.CalculateBillTotalAmount(command.BillKey, ct);

        var paymentResult = await paymentRepository.CreateCashPayment(
            billKey: command.BillKey,
            amount: totalAmount,
            ct);

        if (!paymentResult.TryGetValue(out var payment))
            return paymentResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetPayment(projection, payment, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created payment.");

        return (payment, response);
    }

    public async Task<ResultObject<(PaymentKey, TDto)>> CreateStripePayment<TDto>(
        Expression<Func<StripePayment, TDto>> projection,
        StripePaymentCreateCommand command,
        CancellationToken ct = default)
    {
        var paymentResult = await paymentRepository.CreateStripePayment(
            billKey: command.BillKey,
            amount: command.Amount,
            sessionId: command.SessionId,
            ct);

        if (!paymentResult.TryGetValue(out var payment))
            return paymentResult.Errors;

        var totalAmount = await calculatorService.CalculateBillTotalAmount(command.BillKey, ct);

        if (totalAmount != command.Amount)
        {
            payment.Status = PaymentStatus.Failed;
        }

        await persistenceService.Commit(ct);

        var response = await GetPayment(projection, payment, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created payment.");

        return (payment, response);
    }

    public async Task HandleStripePaymentWebhook(
        PaymentKey paymentKey, CancellationToken ct = default)
    {
        var payment = await paymentRepository.GetPayment(paymentKey, ct);

        if (payment is null)
            return;

        payment.Status = PaymentStatus.Succeeded;

        await publishEndpoint.Publish(
            new PaymentStatusUpdatedMessage
            {
                Resource = payment,
                Status = payment.Status,
            },
            ct);

        await persistenceService.Commit(ct);
    }

    public async Task<TDto?> GetPayment<TDto, TPayment>(
        Expression<Func<TPayment, TDto>> projection, PaymentKey key,
        CancellationToken ct = default) where TPayment : Payment
    {
        return await paymentRepository.QuerySinglePayment(key)
            .OfType<TPayment>()
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<TDto?> GetPayment<TDto>(
        Expression<Func<Payment, TDto>> projection, PaymentKey key,
        CancellationToken ct = default)
    {
        return await paymentRepository.QuerySinglePayment(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdatePayment(
        PaymentKey key, PaymentStatus status,
        CancellationToken ct = default)
    {
        var payment = await paymentRepository.GetPayment(key, ct);

        if (payment is null)
            return ResultObject.NotFound(key);

        payment.Status = status;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}