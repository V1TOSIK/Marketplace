using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Validations;
using UserModule.Application.User.Commands.UpdateUser;

namespace UserModule.Application.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddUserApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationInjection).Assembly));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblyContaining<UpdateUserCommandValidator>();

            return services;
        }
    }
}
