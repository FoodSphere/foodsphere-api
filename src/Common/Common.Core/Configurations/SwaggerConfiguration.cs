using System.Reflection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FoodSphere.Common.Configuration;

public static class SwaggerGenConfiguration
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

            options.MapType<short>(() => new OpenApiSchema
            {
                Type = JsonSchemaType.Integer,
                Example = 1
            });

            options.MapType<int>(() => new OpenApiSchema
            {
                Type = JsonSchemaType.Integer,
                Example = 1
            });

            options.IncludeXmlComments(xmlPath);
        };
    }
}

public static class SwaggerConfiguration
{
    public static Action<SwaggerOptions> Configure()
    {
        return options =>
        {
            options.PreSerializeFilters.Add((doc, req) =>
            {
                var prefix = req.Headers["X-Forwarded-Prefix"].FirstOrDefault("");

                doc.Servers =
                [
                    new() { Url = $"{req.Scheme}://{req.Host.Value}{prefix}" }
                ];
            });
        };
    }
}

// app.UseForwardedHeaders(new ForwardedHeadersOptions
// {
//     ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
// });



// Read [](file:///home/shoguncoffee/.dotnet/symbolcache/JsonNode.cs)

// **JsonNode** is part of the `System.Text.Json.Nodes` API (introduced in .NET 6), which allows you to work with a **mutable** JSON document object model (DOM).

// ### 1. What is `JsonNode`?
// It is the abstract base class for a node in the JSON tree. It has three main concrete implementations:
// *   **`JsonObject`**: Represents a JSON object `{ "prop": "value" }`.
// *   **`JsonArray`**: Represents a JSON array `[1, 2, 3]`.
// *   **`JsonValue`**: Represents a specific primitive value (string, number, boolean, null).

// Unlike `JsonElement` (which is read-only), you can modify a `JsonNode` after creating it.

// ### 2. Why can you use `Example = 1`?
// You can assign an integer directly to a `JsonNode` property because the class includes **implicit conversion operators**.

// Even though `type` of the `Example` property is `JsonNode?`, the library defines an implicit converter that automatically wraps your primitive integer `1` into a `JsonValue`.

// Essentially, the compiler translates your code from this:
// ```csharp
// Example = 1
// ```
// To this:
// ```csharp
// Example = JsonValue.Create(1)
// ```

// This syntax sugar is provided for most primitive types (`int`, `string`, `bool`, `double`, etc.) to make constructing JSON objects more natural and less verbose.