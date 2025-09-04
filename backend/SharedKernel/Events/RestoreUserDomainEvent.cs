using MediatR;

namespace SharedKernel.Events
{
    public class RestoreUserDomainEvent : INotification
    {
        public Guid UserId { get; }
        public RestoreUserDomainEvent(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }
    }
}
