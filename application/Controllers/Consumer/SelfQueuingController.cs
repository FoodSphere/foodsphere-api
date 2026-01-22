using Microsoft.AspNetCore.Mvc;
using FoodSphere.Services;

namespace FoodSphere.Controllers.Consumer;

[Route("consumer/queue")]
public class SelfQueuingController(
    ILogger<SelfQueuingController> logger,
    BillService billService,
    BranchService branchService
) : ConsumerController
{
    readonly ILogger<SelfQueuingController> _logger = logger;
    readonly BillService _billService = billService;
    readonly BranchService _branchService = branchService;

    // [HttpGet("reserve/{id}")]
    // public async Task<ActionResult> ReserveTable(short id)
    // {
    //     var table = await _branchService.GetTable(id);

    //     if (table is null)
    //     {
    //         return NotFound();
    //     }

    //     // if (table.IsReserved)
    //     // {
    //     //     return BadRequest("Table is already reserved.");
    //     // }

    //     // else
    //     // {
    //     //     table.IsReserved = true;
    //     //     await _tableService.Update(table);
    //     // }
    //     return Ok(table);
    // }
}