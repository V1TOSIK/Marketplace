using MediatR;

namespace SharedKernel.Events
{
    public class SoftDeleteUserDomainEvent : INotification
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
