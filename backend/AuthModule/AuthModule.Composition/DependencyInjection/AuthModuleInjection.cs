using Microsoft.Extensions.Configuration;
using AuthModule.Infrastructure.DependencyInjection;
using AuthModule.Application.DependencyInjection;
using AuthModule.Persistence.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Interfaces;
using SharedKernel.UnitOfWork;
using AuthModule.Persistence;
using SharedKernel.Extensions.DependencyInjection;

namespace AuthModule.Composition.DependencyInjection
{
    public static class AuthModuleInjection
    {
        public static IServiceCollection AddAuthModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(configuration);
            services.AddAuthApplication(configuration);
            services.AddAuthPersistence(configuration);
            services.AddAuthInfrastructure(configuration);

            services.AddModuleInitializer<AuthDbContext>();
            services.AddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
            return services;
        }
    }
}
