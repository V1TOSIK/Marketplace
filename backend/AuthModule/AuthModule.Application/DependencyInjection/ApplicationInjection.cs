using AuthModule.Application.Interfaces.Services;
using AuthModule.Application.Options;
using AuthModule.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddAuthApplication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationInjection).Assembly));
            
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IVerificationService, VerificationService>();


            services.Configure<GoogleOptions>(configuration.GetSection("GOOGLE"));


            return services;
        }
    }
}
