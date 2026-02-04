using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using FoodSphere.Common.Configurations;
using FoodSphere.Infrastructure.Persistence;
using FoodSphere.SelfOrdering.Api.Configurations;
using FoodSphere.SelfOrdering.Api.Authentication;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    DotNetEnv.Env.Load(Path.Combine(AppContext.BaseDirectory, ".env.development"));
    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddSwaggerGen(SwaggerConfiguration.Configure());
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
builder.Services.AddDomainConsumerOptions();
builder.Services.AddDomainOrderingOptions();

builder.Services.AddDbContext<FoodSphereDbContext>((sp, optionsBuilder) => {
    var envConnectionString = sp.GetRequiredService<IOptions<EnvConnectionStrings>>().Value;

    // optionsBuilder.UseLazyLoadingProxies();
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
authBuilder.AddSelfOrderingJwt(builder.Services);

if (builder.Environment.IsProduction())
{
    builder.Services.AddGoogleOptions();
    authBuilder.AddGoogle(GoogleConfiguration.Configure(builder.Services));
}

builder.Services.AddAuthorization(AuthorizationConfiguration.Configure());

builder.Services.AddScoped<SelfOrderingAuthService>();
builder.Services.AddScoped<OrderingPortalService>();
builder.Services.AddScoped<BillService>();
builder.Services.AddScoped<BranchService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<RestaurantService>();
builder.Services.AddScoped<StaffService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();