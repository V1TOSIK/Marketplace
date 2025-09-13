using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class SoftDeleteUserEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public SoftDeleteUserEvent(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }
    }
}
