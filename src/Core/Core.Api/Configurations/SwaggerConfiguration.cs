using System.Reflection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FoodSphere.Core.Api.Configurations;

public static class SwaggerConfiguration
{
    public const string JwtSchemeName = "Bearer";

    public static Action<SwaggerGenOptions> Configure()
    {
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

            var assemblyName = Assembly.GetEntryAssembly()!.GetName();
            var xmlFileName = $"{assemblyName.Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFileName);

            options.IncludeXmlComments(xmlPath);
        };
    }
}