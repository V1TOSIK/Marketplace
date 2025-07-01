using AuthModule.Persistence;
using Marketplace.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthModule.Composition
{
    public class AuthModuleInitializer : IModuleInitializer
    {
        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            await context.Database.MigrateAsync();
        }
    }
}
