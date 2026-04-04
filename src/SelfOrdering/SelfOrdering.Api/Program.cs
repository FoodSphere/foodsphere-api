using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.AspNetCore.Identity;
using MassTransit;
using FoodSphere.Common.Configuration;
using FoodSphere.Infrastructure.Extension;
using FoodSphere.Infrastructure.Persistence;
using FoodSphere.SelfOrdering.Api.Configuration;
using FoodSphere.SelfOrdering.Api.Authentication;
using FoodSphere.SelfOrdering.Api.Event;

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
builder.Services.AddDomainConsumerOptions();
builder.Services.AddDomainOrderingOptions();

builder.Services.AddDbContext<FoodSphereDbContext>((sp, optionsBuilder) => {
    var envConnectionString = sp.GetRequiredService<IOptions<EnvConnectionStrings>>().Value;

    optionsBuilder.UseLazyLoadingProxies();
    optionsBuilder.UseNpgsql(envConnectionString.@default, sqlOptions =>
    {
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

builder.Services.AddRepositoryServices();
builder.Services.AddScoped<PersistenceService>();

builder.Services.AddScoped<SelfOrderingAuthService>();
builder.Services.AddScoped<OrderingAuthService>();
builder.Services.AddScoped<OrderingPortalServiceBase>();

// builder.Services.AddScoped<ConsumerServiceBase>();
builder.Services.AddScoped<BillServiceBase>();
builder.Services.AddScoped<BranchServiceBase>();
builder.Services.AddScoped<TableServiceBase>();
builder.Services.AddScoped<MenuServiceBase>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<RestaurantServiceBase>();
builder.Services.AddScoped<WorkerServiceBase>();
builder.Services.AddScoped<StockServiceBase>();
builder.Services.AddScoped<OrderServiceBase>();
builder.Services.AddScoped<TagServiceBase>();
builder.Services.AddScoped<OrderingCalculator>();
builder.Services.AddScoped<ServiceRequestService>();

builder.Services.AddSignalR();
builder.Services.AddMassTransit(MassTransitConfiguration.Configure());
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

app.MapHub<OrderingHub>("hubs/ordering")
    .RequireCors("SignalRPolicy");

app.Run();