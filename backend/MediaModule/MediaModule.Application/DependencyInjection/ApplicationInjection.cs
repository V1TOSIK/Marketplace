using MediaModule.Application.Interfaces.Services;
using MediaModule.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MediaModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddMediaApplication(this IServiceCollection services)
        {
            services.AddScoped<IMediaService, MediaService>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationInjection).Assembly));

            return services;
        }
    }
}
