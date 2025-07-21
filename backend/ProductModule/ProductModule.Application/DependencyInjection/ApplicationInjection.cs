using Microsoft.Extensions.DependencyInjection;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Services;

namespace ProductModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddProductApplication(this IServiceCollection services)
        {

            services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
