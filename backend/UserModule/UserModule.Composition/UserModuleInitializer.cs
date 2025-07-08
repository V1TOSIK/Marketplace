using Marketplace.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserModule.Persistence;

namespace UserModule.Composition
{
    public class UserModuleInitializer : IModuleInitializer
    {
        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            await context.Database.MigrateAsync();
        }
    }
}
