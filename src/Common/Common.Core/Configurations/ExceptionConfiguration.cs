using Microsoft.AspNetCore.Builder;

namespace FoodSphere.Common.Configurations;

public static class ExceptionConfiguration
{
    public static Action<ExceptionHandlerOptions> Configure()
    {
        return options =>
        {
        };
    }
}