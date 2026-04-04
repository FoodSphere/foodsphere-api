using System.Runtime.CompilerServices;

namespace FoodSphere.Pos.Api.Controller;

[Route("current")]
public class CurrentAuthController(
    ILogger<WorkerAuthController> logger,
     AuthorizeHelperService authService
) : PosControllerBase
{
    /// <summary>
    /// list your authorization's info
    /// </summary>
    [HttpGet("authorization")]
    public async Task<ActionResult<CurrentAuthResponse>> GetAuthorization(
        [FromQuery] Guid restaurant_id)
    {
        PermissionResponse[] permissions = [];
        bool isOwner = false;

        if (UserType is UserType.Master)
        {
            if (await authService.IsRestaurantOwner(
                new(restaurant_id), MasterKey))
            {
                isOwner = true;
            }
            else
            {
                permissions = await authService.ListStaffPermissions(
                    new(restaurant_id, MasterKey.Id), PermissionResponse.Projection);
            }
        }
        else if (UserType is UserType.Worker)
        {
            if (WorkerKey.RestaurantId != restaurant_id)
                return Forbid();

            permissions = await authService.ListWorkerPermissions(
                WorkerKey, PermissionResponse.Projection);
        }

        return new CurrentAuthResponse
        {
            is_owner = isOwner,
            permissions = permissions
        };
    }
}