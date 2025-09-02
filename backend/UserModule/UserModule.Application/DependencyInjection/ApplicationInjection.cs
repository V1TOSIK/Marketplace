using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Interfaces;
using UserModule.Application.Interfaces.Services;
using UserModule.Application.Services;

namespace UserModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddUserApplication(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserBlockService, UserBlockService>();
            services.AddScoped<IUserRestorer, UserRestorer>();
            return services;
        }
    }
}
