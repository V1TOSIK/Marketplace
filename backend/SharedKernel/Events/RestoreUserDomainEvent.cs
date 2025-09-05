using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class RestoreUserDomainEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public RestoreUserDomainEvent(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }

        public RestoreUserDomainEvent()
        {
        }
    }
}
