using Marketplace.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Interfaces;
using UserModule.Application.DependencyInjection;
using UserModule.Persistence;
using UserModule.Persistence.DependencyInjection;
using UserModule.Persistence.UnitOfWork;

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
            services.AddScoped<IUserUnitOfWork>(provider =>
            {
                var context = provider.GetRequiredService<UserDbContext>();
                return new UserUnitOfWork<UserDbContext>(context);
            });

            return services;
        }
    }
}
