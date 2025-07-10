using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserModule.Domain.Interfaces;
using UserModule.Persistence.Repositories;

namespace UserModule.Persistence.DependencyInjection
{
    public static class PersistenceInjection
    {
        public static IServiceCollection AddUserPersistence(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("Postgres"));
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserBlockRepository, UserBlockRepository>();


            return services;
        }
    }
}
