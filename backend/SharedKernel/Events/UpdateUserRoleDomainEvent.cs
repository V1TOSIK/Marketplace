using MediatR;

namespace SharedKernel.Events
{
    public class UpdateUserRoleDomainEvent : INotification
    {
        public Guid UserId { get; }
        public string NewRole { get; }
        public UpdateUserRoleDomainEvent(Guid userId, string newRole)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            if (string.IsNullOrWhiteSpace(newRole))
                throw new ArgumentException("New role cannot be empty or null.", nameof(newRole));
            UserId = userId;
            NewRole = newRole;
        }
    }
}
