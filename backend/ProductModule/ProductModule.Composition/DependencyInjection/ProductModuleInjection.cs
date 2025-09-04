using Marketplace.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductModule.Application.DependencyInjection;
using ProductModule.Persistence;
using ProductModule.Persistence.DependencyInjection;
using ProductModule.Persistence.UnitOfWork;
using SharedKernel.Interfaces;

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

            services.AddScoped<IModuleInitializer, ProductModuleInitializer>();
            services.AddScoped<IProductUnitOfWork>(provider =>
            {
                var context = provider.GetRequiredService<ProductDbContext>();
                return new ProductUnitOfWork<ProductDbContext>(context);
            });
            return services;
        }
    }
}
