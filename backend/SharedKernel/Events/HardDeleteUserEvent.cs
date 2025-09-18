using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class HardDeleteUserEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public HardDeleteUserEvent(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }
    }
}
