using Microsoft.AspNetCore.Mvc;
using FoodSphere.Services;
using FoodSphere.Data.DTOs;

namespace FoodSphere.Controllers.Consumer;

[Route("consumer")]
public class SelfTransactionController(
    ILogger<SelfTransactionController> logger,
    BillService billService
) : ConsumerController
{
    readonly ILogger<SelfTransactionController> _logger = logger;
    readonly BillService _billService = billService;

    [HttpGet("bills")]
    public async Task ListBills()
    {
        throw new NotImplementedException();
    }
}