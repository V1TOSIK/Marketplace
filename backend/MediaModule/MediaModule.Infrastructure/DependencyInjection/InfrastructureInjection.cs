using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaModule.Infrastructure.DependencyInjection
{
    public static class InfrastructureInjection
    {
        public static IServiceCollection AddMediaInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMinIo(configuration);
            return services;
        }
    }
}
