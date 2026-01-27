using System.Reflection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FoodSphere.Core.Configurations;

public static class SwaggerConfiguration
{
    public const string JwtSchemeName = "Bearer";

    public static Action<SwaggerGenOptions> Configure()
    {
        // if capture inside lambda: Assembly => Microsoft.Extensions.Options
        // not real calling assembly we want
        var callingAssembly = Assembly.GetCallingAssembly();
        var assemblyName = callingAssembly.GetName();
        var xmlFileName = $"{assemblyName.Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFileName);

        return options =>
        {
            options.AddSecurityDefinition(JwtSchemeName, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Description = "enter JWT token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement()
            {
                [new OpenApiSecuritySchemeReference(JwtSchemeName, doc)] = [] // name must match the SecurityDefinition scheme
            });

            options.IncludeXmlComments(xmlPath);
        };
    }
}