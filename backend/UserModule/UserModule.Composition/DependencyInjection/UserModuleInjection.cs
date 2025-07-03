using Marketplace.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserModule.Application.DependencyInjection;
using UserModule.Persistence.DependencyInjection;

namespace UserModule.Composition.DependencyInjection
{
    public static class UserModuleInjection
    {
        public static IServiceCollection AddUserModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationInjection();
            services.AddPersistenceInjection(configuration);

            services.AddScoped<IModuleInitializer, UserModuleInitializer>();
            return services;
        }
    }
}
