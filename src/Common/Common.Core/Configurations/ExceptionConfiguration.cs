using Microsoft.AspNetCore.Builder;

namespace FoodSphere.Common.Configuration;

public static class ExceptionConfiguration
{
    public static Action<ExceptionHandlerOptions> Configure()
    {
        return options =>
        {
        };
    }
}