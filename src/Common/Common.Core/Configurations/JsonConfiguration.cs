using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.Common.Configurations;

public static class JsonConfiguration
{
    public static Action<JsonOptions> Configure()
    {
        return options =>
        {
            options.JsonSerializerOptions.AllowTrailingCommas = true;
        };
    }
}