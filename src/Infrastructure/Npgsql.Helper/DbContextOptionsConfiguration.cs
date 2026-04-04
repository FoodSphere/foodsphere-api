using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using FoodSphere.Common.Options;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Infrastructure.Npgsql;

public static class NpgsqlDbContextOptionsBuilderExtensions
{
    extension(NpgsqlDbContextOptionsBuilder sqlOptions)
    {
        public NpgsqlDbContextOptionsBuilder FoodSphereConfigure()
        {
            return sqlOptions
                .MigrationsAssembly(typeof(NpgsqlDbContextOptionsBuilderExtensions).Assembly)
                // .UseAdminDatabase(builder.Configuration.GetConnectionString("admin"));
                .EnableRetryOnFailure(2);
        }
    }
}

public static class DbContextConfiguration
{
    public static Action<IServiceProvider, DbContextOptionsBuilder> Configure()
    {
        return (sp, optionsBuilder) => {
            var envConnectionString = sp.GetRequiredService<IOptions<EnvConnectionStrings>>().Value;

            optionsBuilder.UseFoodSphereSeeding(sp);
            optionsBuilder.UseNpgsql(envConnectionString.@default, sqlOptions =>
            {
                sqlOptions.FoodSphereConfigure();
            });

            // if (builder.Environment.IsDevelopment())
            // {
                optionsBuilder.EnableSensitiveDataLogging();
            // }
        };
    }
}