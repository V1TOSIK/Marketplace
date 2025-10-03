using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductModule.Application.DependencyInjection;
using ProductModule.Persistence;
using ProductModule.Persistence.DependencyInjection;
using SharedKernel.Extensions.DependencyInjection;
using SharedKernel.UnitOfWork;

namespace ProductModule.Composition.DependencyInjection
{
    public static class ProductModuleInjection
    {
        public static IServiceCollection AddProductModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddProductApplication();
            services.AddProductPersistence(configuration);

            services.AddModuleInitializer<ProductDbContext>();
            services.AddScoped<IUnitOfWork<ProductDbContext>, UnitOfWork<ProductDbContext>>();
            return services;
        }
    }
}
