using MediaModule.Application.Interfaces;
using MediaModule.Application.Interfaces.Repositories;
using MediaModule.Persistence.Repositories;
using MediaModule.Persistence.UnitOfWork;
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
            services.AddScoped<IMediaUnitOfWork, MediaUnitOfWork>();
            return services;
        }
    }
}
