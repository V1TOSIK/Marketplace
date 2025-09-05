using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class SoftDeleteUserDomainEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public SoftDeleteUserDomainEvent(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }
    }
}
