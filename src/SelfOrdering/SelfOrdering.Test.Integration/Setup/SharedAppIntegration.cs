using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using FoodSphere.Infrastructure.Persistence;
using FoodSphere.Infrastructure.Npgsql;
using FoodSphere.Common.Service;
using FoodSphere.SelfOrdering.Api.Service;

[assembly: AssemblyFixture(typeof(FoodSphere.SelfOrdering.Test.Integration.SharedAppFixture))]

namespace FoodSphere.SelfOrdering.Test.Integration;

// https://github.com/TDMR87/IntegrationTestsInDotnet
public class SharedAppFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    readonly PostgreSqlContainer _default = new PostgreSqlBuilder("postgres:18-alpine")
        // .WithAutoRemove(true)
        // .WithCleanUp(true)
        .Build();

    readonly RabbitMqContainer _rabbitmq = new RabbitMqBuilder("rabbitmq:4-alpine")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        // # ERROR: builder.ConfigureAppConfiguration();
        // in minimal hosting
        // HostApplicationBuilder.Build()
        // -> Transfers state from ConfigurationStaff to ConfigurationRoot
        // -> Disposes ConfigurationStaff
        Environment.SetEnvironmentVariable("ConnectionStrings__default",
            _default.GetConnectionString() + ";Include Error Detail=true");

        Environment.SetEnvironmentVariable("ConnectionStrings__rabbitmq",
            _rabbitmq.GetConnectionString());

        builder.ConfigureTestServices(services =>
        {
            services.AddIdentityCore<MasterUser>(options =>
                {
                    options.Password.RequiredLength = 4;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;

                    options.ClaimsIdentity = FoodSphereClaimType.Identity;
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<FoodSphereDbContext>();
        });
    }

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        await _default.StartAsync(TestContext.Current.CancellationToken);
        await _rabbitmq.StartAsync(TestContext.Current.CancellationToken);

        using var scope = Services.CreateScope();

        var optionsBuilder = new DbContextOptionsBuilder<FoodSphereDbContext>();
        DbContextConfiguration.Configure()(scope.ServiceProvider, optionsBuilder);
        using var dbContext = new FoodSphereDbContext(optionsBuilder.Options);

        await dbContext.Database.MigrateAsync(TestContext.Current.CancellationToken);
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _default.DisposeAsync();
        await _rabbitmq.DisposeAsync();
    }
}

public abstract class SharedAppTestsBase(SharedAppFixture Factory)
{
    protected readonly HttpClient _client = Factory.CreateClient();
    protected IServiceScope CreateScope() => Factory.Services.CreateScope();

    protected TestSeeder CreateTestSeeder(IServiceScope? scope = null, CancellationToken? ct = null)
    {
        return new(scope ?? CreateScope(), disposeScope: scope is null, ct ?? TestContext.Current.CancellationToken);
    }

    protected async Task Authenticate(BillMember member)
    {
        using var scope = CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<OrderingAuthService>();

        var token = await authService.GenerateToken(member);

        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
    }

    protected static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        PropertyNameCaseInsensitive = true
    };
}