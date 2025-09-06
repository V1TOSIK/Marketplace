using MediatR;
using Microsoft.EntityFrameworkCore;
using UserModule.Domain.Entities;

namespace UserModule.Persistence
{
    public class UserDbContext : DbContext
    {
        private readonly IMediator _mediator;
        public UserDbContext(DbContextOptions<UserDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
            _mediator = null!;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserPhoneNumber> UserPhoneNumbers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = ChangeTracker
            .Entries<User>()
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
