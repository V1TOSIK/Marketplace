using AuthModule.Application.Interfaces;
using AuthModule.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddAuthApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();


            return services;
        }
    }
}
