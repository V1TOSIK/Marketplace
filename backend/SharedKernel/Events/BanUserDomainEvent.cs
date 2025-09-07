using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class BanUserDomainEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public string BanReason { get; } = "Violation of terms of service";

        public BanUserDomainEvent(Guid userId, string? banReason)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
            if (!string.IsNullOrWhiteSpace(banReason))
                BanReason = banReason;
        }
    }
}
