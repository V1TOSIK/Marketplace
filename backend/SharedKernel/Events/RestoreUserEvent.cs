using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class RestoreUserEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public RestoreUserEvent(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }

        public RestoreUserEvent()
        {
        }
    }
}
