using Microsoft.Extensions.DependencyInjection;
using ProductModule.Application.Interfaces;

namespace ProductModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddProductApplication(this IServiceCollection services)
        {

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationInjection).Assembly));

            return services;
        }
    }
}
