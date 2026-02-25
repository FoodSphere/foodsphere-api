using Microsoft.AspNetCore.Authentication.Google;

namespace FoodSphere.Pos.Api.Configuration;

public static class GoogleConfiguration
{
    public static Action<GoogleOptions> Configure(IServiceCollection services)
    {
        using var sp = services.BuildServiceProvider();
        var envGoogle = sp.GetRequiredService<IOptions<EnvGoogle>>().Value;

        return options => {
            options.ClientId = envGoogle.client_id;
            options.ClientSecret = envGoogle.client_secret;
        };
    }
}