using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using FoodSphere.Common.Configuration;
using FoodSphere.Infrastructure.Persistence;
using FoodSphere.Pos.Api.Utility;
using FoodSphere.Pos.Api.Configuration;
using FoodSphere.Pos.Api.Authentication;

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

    // # navigation properties, Eager vs Explicit vs Lazy
    // ## Eager loading
    // ```
    // var blog = context.Blogs
    //     .Include(b => b.Posts)
    //     .FirstOrDefault(b => b.Id == id);
    // ```

    // ## Explicit loading
    // ```
    // var blog = context.Blogs.Find(id);
    // context.Entry(blog).Collection(b => b.Posts).Load();
    // ```
    optionsBuilder.UseLazyLoadingProxies();
    optionsBuilder.UseNpgsql(envConnectionString.@default, sqlOptions =>
    {
        // sqlOptions.EnableRetryOnFailure(2);
    });

    if (builder.Environment.IsDevelopment())
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        optionsBuilder.EnableSensitiveDataLogging();
    }
});

// https://github.com/dotnet/aspnetcore/blob/8f657272b6a9092f58df84c0123729919a693fbe/src/Identity/Extensions.Core/src/IdentityServiceCollectionExtensions.cs#L23
// services.TryAddScoped<UserManager<TUser>>(); register by AddIdentityCore:
builder.Services.AddIdentityCore<MasterUser>(IdentityConfiguration.Configure())
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders() /// map token providers name to handler type eg. "Email" -> EmailTokenProvider (TOTP) <see cref="EmailTokenProvider{TUser}"/>
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

// lifetime of the application
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<MimeService>();

builder.Services.AddSingleton<Amazon.S3.IAmazonS3>(S3Configuration.Configure);
builder.Services.AddScoped<IStorageService, S3StorageService>();

// scoped each http request
builder.Services.AddScoped<IAuthorizationHandler, RestaurantPermissionHandler>();
builder.Services.AddScoped<IAuthorizationHandler, BranchPermissionHandler>();

builder.Services.AddScoped<MasterAuthService>();
builder.Services.AddScoped<StaffAuthService>();
builder.Services.AddScoped<OrderingAuthService>();
builder.Services.AddScoped<AuthorizeService>();
builder.Services.AddScoped<AccessControlService>();
builder.Services.AddScoped<OrderingPortalService>();
builder.Services.AddScoped<StaffPortalService>();

builder.Services.AddScoped<RestaurantService>();
builder.Services.AddScoped<BranchService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<MenuImageService>();
builder.Services.AddScoped<IngredientService>();
builder.Services.AddScoped<IngredientService>();
builder.Services.AddScoped<IngredientImageService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<StaffService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<BillService>();
builder.Services.AddScoped<PaymentService>();

// short-lived each injection used
// AddKeyedTransient?
builder.Services.AddTransient<IMagicLinkService, MessagePackMagicLinkService>();

builder.Services.AddControllers()
    .AddJsonOptions(JsonConfiguration.Configure());

builder.Services.AddEndpointsApiExplorer();
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

HealthCheck.Check(app);

app.Run();