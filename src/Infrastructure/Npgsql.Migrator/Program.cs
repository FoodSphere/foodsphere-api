using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.Extensions.Options;
using FoodSphere.Common.Configuration;
using FoodSphere.Common.Options;
using FoodSphere.Infrastructure.Persistence;
using FoodSphere.Infrastructure.Npgsql;
using FoodSphere.Worker.Migration;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    DotNetEnv.Env.NoClobber().Load(Path.Combine(AppContext.BaseDirectory, ".env.development"));
    builder.Configuration.AddEnvironmentVariables();
    builder.AddServiceDefaults();
}
else if (builder.Environment.IsProduction())
{
    builder.Services.AddKeyVaultOptions();

    using var sp = builder.Services.BuildServiceProvider();
    {
        var envKeyVault = sp.GetRequiredService<IOptions<EnvKeyVault>>().Value;

        builder.Configuration.AddAzureKeyVault(
            new Uri(envKeyVault.uri),
            new DefaultAzureCredential(),
            new AzureKeyVaultConfigurationOptions { ReloadInterval = TimeSpan.FromMinutes(30) });
    }
}
else
{
    throw new InvalidOperationException("unsupported environment");
}

builder.Services.AddConnectionStringsOptions();
builder.Services.AddDbContext<FoodSphereDbContext>(DbContextConfiguration.Configure());

builder.Services.AddHostedService<NpgsqlMigrationWorker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(NpgsqlMigrationWorker.ActivitySourceName));

var host = builder.Build();
host.Run();