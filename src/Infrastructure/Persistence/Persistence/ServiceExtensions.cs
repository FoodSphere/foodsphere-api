using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace FoodSphere.Infrastructure.Extension;

public static class RepositoryServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRepositoryServices()
        {
            var repoBaseType = typeof(Repository.RepositoryBase);

            var types = repoBaseType.Assembly.GetTypes()
                .Where(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    type.Namespace == repoBaseType.Namespace &&
                    type.IsAssignableTo(repoBaseType));

            foreach (var type in types)
                services.AddScoped(type);

            services.AddScoped<IPasswordHasher<MasterUser>, PasswordHasher<MasterUser>>();
            services.AddScoped<IPasswordHasher<ConsumerUser>, PasswordHasher<ConsumerUser>>();

            return services;
        }
    }
}