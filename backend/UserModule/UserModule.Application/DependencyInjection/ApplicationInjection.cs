using Microsoft.Extensions.DependencyInjection;

namespace UserModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddUserApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationInjection).Assembly));

            return services;
        }
    }
}
