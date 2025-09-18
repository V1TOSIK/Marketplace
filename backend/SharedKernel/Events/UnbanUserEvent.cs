using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class UnbanUserEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public UnbanUserEvent(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }
    }
}
