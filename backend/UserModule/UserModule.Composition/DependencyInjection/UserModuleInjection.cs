using Marketplace.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Interfaces;
using SharedKernel.UnitOfWork;
using UserModule.Application.DependencyInjection;
using UserModule.Persistence;
using UserModule.Persistence.DependencyInjection;

namespace UserModule.Composition.DependencyInjection
{
    public static class UserModuleInjection
    {
        public static IServiceCollection AddUserModule(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddUserApplication();
            services.AddUserPersistence(configuration);

            services.AddScoped<IModuleInitializer, UserModuleInitializer>();
            services.AddScoped<IUnitOfWork<UserDbContext>, UnitOfWork<UserDbContext>>();

            return services;
        }
    }
}
