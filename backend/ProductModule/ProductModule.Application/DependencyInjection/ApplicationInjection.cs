using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Validations;

namespace ProductModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddProductApplication(this IServiceCollection services)
        {

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationInjection).Assembly));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
