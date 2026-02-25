using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.Common.Configuration;

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