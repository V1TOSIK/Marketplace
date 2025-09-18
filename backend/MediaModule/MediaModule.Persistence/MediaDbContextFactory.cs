using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MediaModule.Persistence
{
    public class MediaDbContextFactory : IDesignTimeDbContextFactory<MediaDbContext>
    {
        public MediaDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<MediaDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("Postgres"));

            return new MediaDbContext(optionsBuilder.Options);
        }
    }
}
