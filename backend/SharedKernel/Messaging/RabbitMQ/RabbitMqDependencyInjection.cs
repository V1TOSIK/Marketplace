using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SharedKernel.Messaging.RabbitMQ
{
    public static class RabbitMqDependencyInjection
    {
        public static IServiceCollection AddRabbitMqService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection("RABBITMQ").Bind);

            services.AddSingleton<IRabbitMqService>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                var service = new RabbitMqService(options.Host, options.Username, options.Password);
                service.InitializeAsync().GetAwaiter().GetResult();
                return service;
            });

            return services;
        }
    }
}
