using Microsoft.AspNetCore.Mvc.Filters;

namespace FoodSphere.Pos.Api.Filter;

// but Attribute can only use compliled time constant values
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AuthFilterAttribute(
    ILogger<AuthFilterAttribute> logger,
    AccessControlService accessControlService
) : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var restaurant_id = context.RouteData.Values["restaurant_id"];
        var branch_id = context.RouteData.Values["branch_id"];
        var bill_id = context.RouteData.Values["bill_id"];

        context.Result = new ForbidResult();
    }
}