using Marketplace.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductModule.Persistence;

namespace ProductModule.Composition
{
    class ProductModuleInitializer : IModuleInitializer
    {
        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
            await context.Database.MigrateAsync();
        }
    }
}
