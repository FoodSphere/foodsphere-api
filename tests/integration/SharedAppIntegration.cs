using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using FoodSphere.Services;
using FoodSphere.Data.Models;

[assembly: AssemblyFixture(typeof(FoodSphere.Tests.Integration.SharedAppFixture))]

namespace FoodSphere.Tests.Integration;

// https://github.com/TDMR87/IntegrationTestsInDotnet
public class SharedAppFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:18-alpine").Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        Environment.SetEnvironmentVariable("ConnectionStrings:default",
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
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
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

    protected TestSeedingBuilder CreateSeedingBuilder(IServiceScope? scope = null, CancellationToken? cancellationToken = null)
    {
        return new(scope ?? CreateScope(), disposeScope: scope is null, cancellationToken ?? TestContext.Current.CancellationToken);
    }

    protected async Task Authenticate(MasterUser user)
    {
        using var scope = CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<MasterAuthService>();

        var token = await authService.GenerateToken(user);

        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
    }

    protected static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        PropertyNameCaseInsensitive = true
    };
}