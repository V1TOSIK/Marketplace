using Microsoft.Extensions.DependencyInjection;
using ProductModule.Application.Interfaces;
using ProductModule.Infrastructure.Services;

namespace ProductModule.Infrastructure.DependencyInjection
{
    public static class InfrastructureInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // maybe need add httpaccessor for current user service
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }
    }
}
