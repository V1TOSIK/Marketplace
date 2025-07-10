using Microsoft.Extensions.Configuration;
using AuthModule.Infrastructure.DependencyInjection;
using AuthModule.Application.DependencyInjection;
using AuthModule.Persistence.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Marketplace.Abstractions;
using SharedKernel.Interfaces;
using AuthModule.Persistence;

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

            services.AddScoped<IModuleInitializer, AuthModuleInitializer>();
            services.AddScoped<IAuthUnitOfWork>(provider =>
            {
                var context = provider.GetRequiredService<AuthDbContext>();
                return new AuthUnitOfWork<AuthDbContext>(context);
            });
            return services;
        }
    }
}
