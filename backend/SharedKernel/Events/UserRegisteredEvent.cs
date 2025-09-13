using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class UserRegisteredEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public UserRegisteredEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}
