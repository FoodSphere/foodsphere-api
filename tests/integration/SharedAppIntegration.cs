using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

[assembly: AssemblyFixture(typeof(FoodSphere.Tests.Integration.SharedAppFixture))]

namespace FoodSphere.Tests.Integration;

// mock external dependencies
// https://github.com/TDMR87/IntegrationTestsInDotnet
public class SharedAppFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:18-alpine").Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>((serviceProvider, dbContextOptions) => dbContextOptions
                .UseLazyLoadingProxies()
                .UseNpgsql(_dbContainer.GetConnectionString())
                .UseSeeding(Seeding.Seed(serviceProvider))
                .UseAsyncSeeding(Seeding.SeedAsync(serviceProvider))
            );
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

    public void SetJwtToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
    }

    public static string GetUniqueName()
    {
        return "test_" + Guid.NewGuid().ToString();
    }

    // protected static readonly JsonSerializerOptions JsonSerializerOptions = new()
    // {
    //     // Fail deserialization if members do not match.
    //     // This will prevent us from receiving wrong data from an API response
    //     // and regarding it as successfull result.
    //     UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,

    //     // Ignore case when deserializing JSON to support PascalCase and camelCase
    //     PropertyNameCaseInsensitive = true
    // };
}