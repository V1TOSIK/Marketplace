using Marketplace.Abstractions;
using MediaModule.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediaModule.Composition
{
    public class MediaModuleInitializer : IModuleInitializer
    {
        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MediaDbContext>();
            await context.Database.MigrateAsync();
        }
    }
}
