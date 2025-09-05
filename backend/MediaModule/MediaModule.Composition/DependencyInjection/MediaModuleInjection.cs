using MediaModule.Application.DependencyInjection;
using MediaModule.Infrastructure.DependencyInjection;
using MediaModule.Persistence.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MediaModule.Persistence;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Interfaces;
using SharedKernel.UnitOfWork;
using SharedKernel.Extensions.DependencyInjection;

namespace MediaModule.Composition.DependencyInjection
{
    public static class MediaModuleInjection
    {
        public static IServiceCollection AddMediaModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediaPersistence(configuration);
            services.AddMediaInfrastructure(configuration);
            services.AddMediaApplication();

            services.AddModuleInitializer<MediaDbContext>();

            services.AddScoped<IUnitOfWork<MediaDbContext>, UnitOfWork<MediaDbContext>>();
            return services;
        }
    }
}
