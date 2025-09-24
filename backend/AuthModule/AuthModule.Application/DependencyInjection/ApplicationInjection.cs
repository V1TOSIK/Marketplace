using AuthModule.Application.Auth.Commands.Login;
using AuthModule.Application.Interfaces.Services;
using AuthModule.Application.Options;
using AuthModule.Application.Services;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Validations;

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

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();


            services.Configure<GoogleOptions>(configuration.GetSection("GOOGLE"));


            return services;
        }
    }
}
