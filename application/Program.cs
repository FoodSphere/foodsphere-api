using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using FoodSphere;
using FoodSphere.Data.Models;
using FoodSphere.Services;
using FoodSphere.Utilities;
using FoodSphere.Configurations;
using FoodSphere.Configurations.Options;

ICredentialService credentialService;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    // Testcontainers in dev -> too slow?
    credentialService = new LocalCredentialService(builder.Configuration);
    builder.Services.AddFoodSphereOptions(builder.Configuration);
    builder.Services.AddSingleton<IDataProtectionProvider, EphemeralDataProtectionProvider>();
    builder.Services.AddSwaggerGen(SwaggerConfiguration.Configure());

    // https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-ephemeral
}
else if (builder.Environment.IsProduction())
{
    builder.Services.AddOptions<KeyVaultOption>()
        .Bind(builder.Configuration.GetSection(KeyVaultOption.SectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    credentialService = new AzureCredentialService(builder.Configuration);
    builder.Services.AddFoodSphereOptions(builder.Configuration);

    // Azure Key Vault vs Db, if the app is spread across multiple machines: https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview
    builder.Services.AddDataProtection();
        // .PersistKeysToDbContext<AppDbContext>();
}
else
{
    throw new InvalidOperationException("unsupported environment");
}

builder.Services.AddDbContext<AppDbContext>((serviceProvider, optionsBuilder) => {
    optionsBuilder.UseLazyLoadingProxies(); // navigation properties, Eager vs Explicit vs Lazy
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("default"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(2);
        // sqlOptions.UseAdminDatabase(builder.Configuration.GetConnectionString("admin"));
    });
    optionsBuilder.UseSeeding(Seeding.Seed(serviceProvider));
    optionsBuilder.UseAsyncSeeding(Seeding.SeedAsync(serviceProvider));

    if (builder.Environment.IsDevelopment())
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        optionsBuilder.EnableSensitiveDataLogging();
    }
});

// https://github.com/dotnet/aspnetcore/blob/8f657272b6a9092f58df84c0123729919a693fbe/src/Identity/Extensions.Core/src/IdentityServiceCollectionExtensions.cs#L23
// services.TryAddScoped<UserManager<TUser>>(); register by AddIdentityCore:
builder.Services.AddIdentityCore<MasterUser>(IdentityConfiguration.Configure())
    .AddRoles<IdentityRole>() // services.TryAddScoped<RoleManager<TRole>>();
    .AddDefaultTokenProviders() /// map token providers name to handler type eg. "Email" -> EmailTokenProvider (TOTP) <see cref="EmailTokenProvider{TUser}"/>
    .AddEntityFrameworkStores<AppDbContext>(); // must be after AddRoles

// https://github.com/search?q=repo%3Adotnet%2Faspnetcore+AddAuthentication%28&type=code
builder.Services.AddAuthentication(AuthenticationConfiguration.Configure())
    .AddFoodSpherePolicy(credentialService)
    .AddJwtAdmin(credentialService)
    .AddJwtClient(credentialService)
    .AddJwtConsumer(credentialService)
    .AddJwtOrdering(credentialService);
    // .AddGoogle(GoogleConfiguration.Configure(credentialService));

builder.Services.AddAuthorization(AuthorizationConfiguration.Configure());

// lifetime of the application
builder.Services.AddSingleton(credentialService);
builder.Services.AddSingleton<EmailService>();

// scoped each http request
builder.Services.AddScoped<AdminAuthService>();
builder.Services.AddScoped<MasterAuthService>();
builder.Services.AddScoped<StaffAuthService>();
builder.Services.AddScoped<ConsumerAuthService>();
builder.Services.AddScoped<OrderingAuthService>();

// builder.Services.AddScoped<IdentityService>();
builder.Services.AddScoped<BillService>();
builder.Services.AddScoped<BranchService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<RestaurantService>();
builder.Services.AddScoped<StaffService>();
builder.Services.AddScoped<OrderingService>();
builder.Services.AddScoped<DashboardService>();

builder.Services.AddSingleton<IAuthorizationPolicyProvider, AppPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, ClientHandler>();

// short-lived each injection use
// AddKeyedTransient?
builder.Services.AddTransient<
    IMagicLinkService,
    MessagePackMagicLinkService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddProblemDetails(); // RFC 9457, Result.Problem()
// builder.Services.AddHealthChecks()
//     .AddCheck<HealthCheck>("custom_health_check")
//     ;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// app.UseExceptionHandler();
app.MapControllers();

HealthCheck.Check(app);

app.Run();