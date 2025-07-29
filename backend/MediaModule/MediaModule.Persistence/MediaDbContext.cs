using MediaModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaModule.Persistence
{
    public class MediaDbContext : DbContext
    {
        public MediaDbContext(DbContextOptions<MediaDbContext> options) : base(options) { }

        public DbSet<Media> Medias { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(MediaDbContext).Assembly);
        }
    }
}
