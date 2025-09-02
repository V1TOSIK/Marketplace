using MediatR;

namespace SharedKernel.Events
{
    public class BanUserDomainEvent : INotification
    {
        public Guid UserId { get; }
        public BanUserDomainEvent(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }
    }
}
