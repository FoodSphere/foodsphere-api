using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using FoodSphere.Common.Configuration;
using FoodSphere.Infrastructure.Persistence;
using FoodSphere.Resource.Api.Configuration;
using FoodSphere.Resource.Api.Authentication;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    DotNetEnv.Env.NoClobber().Load(Path.Combine(AppContext.BaseDirectory, ".env.development"));
    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddSwaggerGen(SwaggerGenConfiguration.Configure());
    builder.Services.AddCors(CorsConfiguration.Configure());
}
else if (builder.Environment.IsProduction())
{
    builder.Services.AddKeyVaultOptions();

    #pragma warning disable ASP0000
    using var sp = builder.Services.BuildServiceProvider();
    {
        var envKeyVault = sp.GetRequiredService<IOptions<EnvKeyVault>>().Value;

        builder.Configuration.AddAzureKeyVault(
            new Uri(envKeyVault.uri),
            new DefaultAzureCredential(),
            new AzureKeyVaultConfigurationOptions { ReloadInterval = TimeSpan.FromMinutes(30) });
    }
    #pragma warning restore ASP0000
}
else
{
    throw new InvalidOperationException("unsupported environment");
}

builder.AddServiceDefaults();
builder.Services.AddConnectionStringsOptions();
builder.Services.AddDomainApiOptions();
builder.Services.AddDomainResourceOptions();

builder.Services.AddDbContext<FoodSphereDbContext>((sp, optionsBuilder) => {
    var envConnectionString = sp.GetRequiredService<IOptions<EnvConnectionStrings>>().Value;

    optionsBuilder.UseLazyLoadingProxies();
    optionsBuilder.UseNpgsql(envConnectionString.@default, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(2);
    });

    if (builder.Environment.IsDevelopment())
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        optionsBuilder.EnableSensitiveDataLogging();
    }
});

var authBuilder = builder.Services.AddAuthentication(DefaultAuthentication.Configure(builder.Services));

// authBuilder.AddDefaultPolicyScheme(builder.Services);
authBuilder.AddResourceJwt(builder.Services);

builder.Services.AddAuthorization(AuthorizationConfiguration.Configure());

builder.Services.AddScoped<ResourceAuthService>();
builder.Services.AddScoped<BillService>();
builder.Services.AddScoped<BranchService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<RestaurantService>();
builder.Services.AddScoped<StaffService>();

builder.Services.AddControllers()
    .AddJsonOptions(JsonConfiguration.Configure());

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(SwaggerConfiguration.Configure());
    app.UseSwaggerUI();
    app.UseCors();
}

app.UseHttpsRedirection();
// app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();