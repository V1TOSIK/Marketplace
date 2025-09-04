using MediatR;

namespace SharedKernel.Events
{
    public class UnbanUserDomainEvent : INotification
    {
        public Guid UserId { get; }
        public UnbanUserDomainEvent(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }
    }
}
