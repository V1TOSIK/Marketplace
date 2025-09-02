using AuthModule.Application.Interfaces;
using AuthModule.Infrastructure.Options;
using AuthModule.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductModule.SharedKernel.Interfaces;

namespace AuthModule.Infrastructure.DependencyInjection
{
    public static class InfrastructureInjection
    {
        public static IServiceCollection AddAuthInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection("JWT"));

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IVerificationStore, RedisVerificationStore>();

            services.Configure<EmailOptions>(configuration.GetSection("SMTP"));
            services.AddTransient<IEmailService, MailKitEmailService>();

            services.Configure<SmsOptions>(configuration.GetSection("TWILIO"));
            services.AddTransient<ISmsService, TwilioSmsService>();

            services.AddScoped<ICookieService, CookieService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}
