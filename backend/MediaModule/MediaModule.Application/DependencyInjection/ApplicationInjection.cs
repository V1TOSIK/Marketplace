using MediaModule.Application.Interfaces.Services;
using MediaModule.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Validations;

namespace MediaModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddMediaApplication(this IServiceCollection services)
        {
            services.AddScoped<IMediaService, MediaService>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationInjection).Assembly));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
