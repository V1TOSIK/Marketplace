using Marketplace.Abstractions;
using ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ProductModule.Composition
{
    public class ProductModuleInitializer : IModuleInitializer
    {
        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
            await context.Database.MigrateAsync();
        }
    }
}
