using MediatR;
using SharedKernel.Entity;

namespace SharedKernel.AgregateRoot
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    {
        private readonly List<INotification> _domainEvents = new();

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(INotification domainEvent) => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
