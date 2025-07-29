using MediaModule.Domain.Interfaces;
using MediaModule.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaModule.Persistence.DependencyInjection
{
    public static class PersistenceInjection
    {
        public static IServiceCollection AddMediaPersistence(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<MediaDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("Postgres"));
            });
            services.AddScoped<IMediaRepository, MediaRepository>();
            return services;
        }
    }
}
