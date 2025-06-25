using AuthModule.Application.Interfaces;
using AuthModule.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthModule.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();


            return services;
        }
    }
}
