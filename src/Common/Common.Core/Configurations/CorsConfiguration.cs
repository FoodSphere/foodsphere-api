using Microsoft.AspNetCore.Cors.Infrastructure;

namespace FoodSphere.Common.Configuration;

public static class CorsConfiguration
{
    public static Action<CorsOptions> Configure()
    {
        return options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
            });

            options.AddPolicy("SignalRPolicy", policy =>
            {
                policy.SetIsOriginAllowed(origin => true);
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowCredentials();
            });
        };
    }
}