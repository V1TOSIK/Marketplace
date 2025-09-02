using MediaModule.Application.Interfaces.Services;
using MediaModule.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Interfaces;

namespace MediaModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddMediaApplication(this IServiceCollection services)
        {
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<IMediaManager, MediaManager>();
            return services;
        }
    }
}
