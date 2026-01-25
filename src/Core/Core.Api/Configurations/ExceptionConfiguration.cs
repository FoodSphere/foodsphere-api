using Microsoft.AspNetCore.Builder;

namespace FoodSphere.Core.Api.Configurations;

public static class ExceptionConfiguration
{
    public static Action<ExceptionHandlerOptions> Configure()
    {
        return options =>
        {
        };
    }
}