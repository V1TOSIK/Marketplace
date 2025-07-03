using Microsoft.Extensions.DependencyInjection;
using UserModule.Application.Interfaces;
using UserModule.Application.Services;

namespace UserModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddApplicationInjection(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
