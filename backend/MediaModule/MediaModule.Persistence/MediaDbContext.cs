using MediaModule.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.AgregateRoot;

namespace MediaModule.Persistence
{
    public class MediaDbContext : DbContext
    {
        private readonly IMediator _mediator;
        public MediaDbContext(DbContextOptions<MediaDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public MediaDbContext(DbContextOptions<MediaDbContext> options) : base(options)
        {
            _mediator = null!;
        }

        public DbSet<Media> Medias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(MediaDbContext).Assembly);
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
