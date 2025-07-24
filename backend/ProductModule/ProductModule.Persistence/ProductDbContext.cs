using Microsoft.EntityFrameworkCore;
using ProductModule.Domain.Entities;

namespace ProductModule.Persistence
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CharacteristicGroup> CharacteristicGroups { get; set; }
        public DbSet<CharacteristicTemplate> CharacteristicTemplates { get; set; }
        public DbSet<CharacteristicValue> CharacteristicValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
        }
    }
}
