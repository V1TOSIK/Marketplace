using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.ModuleInitializer;

namespace SharedKernel.Extensions.DependencyInjection
{
    public static class ModuleInitializerExtensions
    {
        public static IServiceCollection AddModuleInitializer<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            return services.AddScoped<IModuleInitializer, ModuleInitializer<TContext>>();
        }
    }
}
