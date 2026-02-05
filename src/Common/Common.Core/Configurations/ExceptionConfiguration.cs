using Microsoft.AspNetCore.Builder;

namespace FoodSphere.Core.Configurations;

public static class ExceptionConfiguration
{
    public static Action<ExceptionHandlerOptions> Configure()
    {
        return options =>
        {
        };
    }
}