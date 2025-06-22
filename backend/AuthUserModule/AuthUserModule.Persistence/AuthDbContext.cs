using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AuthUserModule.Domain.Entities;

namespace AuthUserModule.Persistence
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        }
    }
}
