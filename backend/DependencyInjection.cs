using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AuthUserModule.Application.Services;
using AuthUserModule.Domain.Interfaces;
using AuthUserModule.Persistence.Repositories;

namespace AuthUserModule;

public class DependencyInjection
{
    public static IServiceCollection AddAuthUserModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("Postgres")));

        services.AddScoped<IAuthUserRepository, AuthUserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
