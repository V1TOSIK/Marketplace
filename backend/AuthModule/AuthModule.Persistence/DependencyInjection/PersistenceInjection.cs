using AuthModule.Domain.Interfaces;
using AuthModule.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthModule.Persistence.DependencyInjection
{
    public static class PersistenceInjection
    {
        public static IServiceCollection AddAuthPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("Postgres"));
            });

            services.AddScoped<IAuthUserRepository, AuthUserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


            return services;
        }
    }
}