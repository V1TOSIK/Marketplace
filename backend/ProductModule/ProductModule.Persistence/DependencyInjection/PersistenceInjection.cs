using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductModule.Persistence.Repositories;
using ProductModule.Domain.Interfaces;

namespace ProductModule.Persistence.DependencyInjection
{
    public static class PersistenceInjection
    {
        public static IServiceCollection AddProductPersistence(this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICharacteristicRepository, CharacteristicRepository>();

            services.AddDbContext<ProductDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("Postgres"));
            });
            return services;
        }
    }
}
