using AuthModule.Application.Interfaces.Services;
using AuthModule.Application.Options;
using AuthModule.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Interfaces;

namespace AuthModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddAuthApplication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IVerificationService, VerificationService>();

            services.Configure<GoogleOptions>(configuration.GetSection("GOOGLE"));


            return services;
        }
    }
}
