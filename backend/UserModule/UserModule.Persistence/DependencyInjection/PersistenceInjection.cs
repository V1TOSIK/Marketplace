using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;
using UserModule.Persistence.Repositories;
using UserModule.Persistence.UnitOfWork;

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
            services.AddScoped<IUserUnitOfWork, UserUnitOfWork>();


            return services;
        }
    }
}
