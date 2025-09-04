using MediaModule.Application.DependencyInjection;
using MediaModule.Infrastructure.DependencyInjection;
using MediaModule.Persistence.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MediaModule.Persistence;
using SharedKernel.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Marketplace.Abstractions;
using MediaModule.Persistence.UnitOfWork;

namespace MediaModule.Composition.DependencyInjection
{
    public static class MediaModuleInjection
    {
        public static IServiceCollection AddMediaModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediaPersistence(configuration);
            services.AddMediaInfrastructure(configuration);
            services.AddMediaApplication();

            services.AddScoped<IModuleInitializer, MediaModuleInitializer>();

            services.AddScoped<IMediaUnitOfWork>(provider =>
            {
                var context = provider.GetRequiredService<MediaDbContext>();
                return new MediaUnitOfWork<MediaDbContext>(context);
            });
            return services;
        }
    }
}
