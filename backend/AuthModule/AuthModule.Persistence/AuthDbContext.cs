using Microsoft.EntityFrameworkCore;
using AuthModule.Domain.Entities;
using MediatR;
using SharedKernel.AgregateRoot;

namespace AuthModule.Persistence
{
    public class AuthDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public AuthDbContext(DbContextOptions<AuthDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
            _mediator = null!;
        }

        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<ExternalLogin> ExternalLogins { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = ChangeTracker
                .Entries<IAggregateRoot>()
                .Where(x => x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            domainEntities.ForEach(e => e.Entity.ClearDomainEvents());

            return result;
        }
    }
}
