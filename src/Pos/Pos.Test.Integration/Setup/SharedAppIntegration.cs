using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using FoodSphere.Infrastructure.Persistence;
using FoodSphere.Pos.Api.Service;

[assembly: AssemblyFixture(typeof(FoodSphere.Pos.Test.Integration.SharedAppFixture))]

namespace FoodSphere.Pos.Test.Integration;

// https://github.com/TDMR87/IntegrationTestsInDotnet
public class SharedAppFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:18-alpine")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        Environment.SetEnvironmentVariable("ConnectionStrings__default",
            _dbContainer.GetConnectionString() + ";Include Error Detail=true"
        );

        builder.ConfigureTestServices(services =>
        {
            // override service registrations, mock external dependencies
        });
    }

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        await _dbContainer.StartAsync(TestContext.Current.CancellationToken);

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FoodSphereDbContext>();

        await dbContext.Database.MigrateAsync(TestContext.Current.CancellationToken);
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _dbContainer.DisposeAsync();
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

    protected async Task Authenticate(MasterUser user)
    {
        using var scope = CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<MasterAuthService>();

        var token = await authService.GenerateToken(user);

        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
    }

    protected async Task Authenticate(StaffUser user)
    {
        using var scope = CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<StaffAuthService>();

        var token = await authService.GenerateToken(user);

        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
    }

    protected static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        PropertyNameCaseInsensitive = true
    };
}