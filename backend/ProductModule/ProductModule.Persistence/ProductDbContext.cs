using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductModule.Domain.Entities;

namespace ProductModule.Persistence
{
    public class ProductDbContext : DbContext
    {
        private readonly IMediator _mediator;
        public ProductDbContext(DbContextOptions<ProductDbContext> options, Mediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
            _mediator = null!;
        }

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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = ChangeTracker
            .Entries<Product>()
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
