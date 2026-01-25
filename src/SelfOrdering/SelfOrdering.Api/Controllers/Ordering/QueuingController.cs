using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.SelfOrdering.Api.Controllers;

// [Route("queue")]
// public class QueuingController(
//     ILogger<QueuingController> logger,
//     BillService billService,
//     BranchService branchService
// ) : SelfOrderingControllerBase
// {
    // [HttpGet("reserve/{id}")]
    // public async Task<ActionResult> ReserveTable(short id)
    // {
    //     var table = await branchService.GetTable(id);

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
    //     //     await tableService.Update(table);
    //     // }
    //     return Ok(table);
    // }
// }