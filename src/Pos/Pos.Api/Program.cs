using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using MassTransit;
using FoodSphere.Common.Configuration;
using FoodSphere.Infrastructure.Persistence;
using FoodSphere.Infrastructure.Extension;
using FoodSphere.Pos.Api.Utility;
using FoodSphere.Pos.Api.Configuration;
using FoodSphere.Pos.Api.Authentication;
using FoodSphere.Pos.Api.Event;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddHealthChecks() vs app.MapHealthChecks() vs app.MapdefaultEndpoints()
builder.AddServiceDefaults();

// builder.Services.AddHealthChecks()
//     .AddCheck<HealthCheck>("custom_health_check");

if (builder.Environment.IsDevelopment())
{
    DotNetEnv.Env.NoClobber().Load(Path.Combine(AppContext.BaseDirectory, ".env.development"));
    builder.Configuration.AddEnvironmentVariables();

    // https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-ephemeral
    // builder.Services.AddSingleton<IDataProtectionProvider, EphemeralDataProtectionProvider>();

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

    // Azure Key Vault vs Db, if the app is spread across multiple machines: https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview
    // builder.Services.AddDataProtection();
        // .PersistKeysToDbContext<AppDbContext>();
}
else
{
    throw new InvalidOperationException("unsupported environment");
}

builder.Services.AddConnectionStringsOptions();
builder.Services.AddS3Options();
builder.Services.AddDomainApiOptions();
builder.Services.AddDomainMasterOptions();
builder.Services.AddDomainPosOptions();

builder.Services.AddDbContext<FoodSphereDbContext>((sp, optionsBuilder) => {
    var envConnectionString = sp.GetRequiredService<IOptions<EnvConnectionStrings>>().Value;
    optionsBuilder.UseNpgsql(envConnectionString.@default, sqlOptions =>
    {
    });

    if (builder.Environment.IsDevelopment())
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        optionsBuilder.EnableSensitiveDataLogging();
    }
});

// https://github.com/dotnet/aspnetcore/blob/8f657272b6a9092f58df84c0123729919a693fbe/src/Identity/Extensions.Core/src/IdentityServiceCollectionExtensions.cs#L23
// services.TryAddScoped<UserStaff<TUser>>(); register by AddIdentityCore:
builder.Services.AddIdentityCore<MasterUser>(IdentityConfiguration.Configure())
    .AddRoles<IdentityRole>()
    // map token providers name to handler type eg. "Email" -> EmailTokenProvider (TOTP)
    /// <see cref="EmailTokenProvider{TUser}"/>
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<FoodSphereDbContext>(); // must be after AddRoles

// https://github.com/search?q=repo%3Adotnet%2Faspnetcore+AddAuthentication%28&type=code
var authBuilder = builder.Services.AddAuthentication(DefaultAuthentication.Configure(builder.Services));

// authBuilder.AddDefaultPolicyScheme(builder.Services);
authBuilder.AddPosJwt(builder.Services);

if (builder.Environment.IsProduction())
{
    builder.Services.AddGoogleOptions();
    authBuilder.AddGoogle(GoogleConfiguration.Configure(builder.Services));
}

builder.Services.AddAuthorization(AuthorizationConfiguration.Configure());

builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<MimeService>();

builder.Services.AddSingleton<Amazon.S3.IAmazonS3>(S3Configuration.Configure);

builder.Services.AddRepositoryServices();
builder.Services.AddScoped<PersistenceService>();
builder.Services.AddScoped<IStorageService, S3StorageService>();

builder.Services.AddScoped<IAuthorizationHandler, RestaurantPermissionHandler>();
builder.Services.AddScoped<IAuthorizationHandler, BranchPermissionHandler>();
builder.Services.AddScoped<IPasswordHasher<ConsumerUser>, PasswordHasher<ConsumerUser>>();

builder.Services.AddScoped<MasterAuthService>();
builder.Services.AddScoped<WorkerAuthService>();
builder.Services.AddScoped<OrderingAuthService>();
builder.Services.AddScoped<AuthorizeHelperService>();
builder.Services.AddScoped<AccessControlService>();
builder.Services.AddScoped<OrderingPortalServiceBase>();
builder.Services.AddScoped<WorkerPortalService>();

// builder.Services.AddScoped<ConsumerServiceBase>();
builder.Services.AddScoped<RestaurantServiceBase>();
builder.Services.AddScoped<RestaurantImageService>();
builder.Services.AddScoped<BranchServiceBase>();
builder.Services.AddScoped<TableServiceBase>();
builder.Services.AddScoped<MenuServiceBase>();
builder.Services.AddScoped<MenuServiceBase>();
builder.Services.AddScoped<MenuImageService>();
builder.Services.AddScoped<IngredientServiceBase>();
builder.Services.AddScoped<IngredientImageService>();
builder.Services.AddScoped<TagServiceBase>();
builder.Services.AddScoped<PermissionServiceBase>();
builder.Services.AddScoped<RoleServiceBase>();
builder.Services.AddScoped<WorkerServiceBase>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<BillServiceBase>();
builder.Services.AddScoped<OrderServiceBase>();
builder.Services.AddScoped<StockServiceBase>();
builder.Services.AddScoped<RestaurantStaffServiceBase>();
builder.Services.AddScoped<BranchStaffServiceBase>();
builder.Services.AddScoped<OrderingCalculator>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<ServiceRequestService>();
builder.Services.AddScoped<ReportCalculator>();
builder.Services.AddScoped<AuthorizeService>();

// short-lived each injection used
// AddKeyedTransient?
builder.Services.AddTransient<IMagicLinkService, MessagePackMagicLinkService>();

builder.Services.AddControllers()
    .AddJsonOptions(JsonConfiguration.Configure());

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR();
builder.Services.AddMassTransit(MassTransitConfiguration.Configure());
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
// builder.Services.AddProblemDetails(); // RFC 9457, Result.Problem()
// builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(SwaggerConfiguration.Configure());
    app.UseSwaggerUI();
    app.UseCors();
    // app.MapOpenApi();
}

app.UseHttpsRedirection();
// app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-10.0?view=aspnetcore-10.0#support-for-server-sent-events-sse
app.MapHub<PosHub>("restaurants/{restaurant_id}/branches/{branch_id}/hubs/pos")
    .RequireCors("SignalRPolicy");

HealthCheck.Check(app);

app.Run();