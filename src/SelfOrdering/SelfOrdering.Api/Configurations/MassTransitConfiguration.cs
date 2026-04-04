using FoodSphere.Infrastructure.Persistence;
using MassTransit;

namespace FoodSphere.SelfOrdering.Api.Configuration;

public static class MassTransitConfiguration
{
    public static Action<IBusRegistrationConfigurator> Configure()
    {
        return busConfig =>
        {
            busConfig.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("ordering", false));
            busConfig.AddConsumersFromNamespaceContaining<Event.OrderingHub>();

            busConfig.AddEntityFrameworkOutbox<FoodSphereDbContext>(config =>
            {
                config.UsePostgres();
                config.UseBusOutbox();
            });

            busConfig.UsingRabbitMq((context, config) =>
            {
                var env = context.GetRequiredService<IOptions<EnvConnectionStrings>>().Value;

                config.Host(env.rabbitmq);
                config.ConfigureEndpoints(context);
            });
        };
    }
}