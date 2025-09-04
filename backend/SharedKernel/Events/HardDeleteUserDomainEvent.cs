using MediatR;

namespace SharedKernel.Events
{
    public class HardDeleteUserDomainEvent : INotification
    {
        public Guid UserId { get; }
        public HardDeleteUserDomainEvent(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }
    }
}
