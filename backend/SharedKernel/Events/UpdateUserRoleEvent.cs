using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class UpdateUserRoleEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public string NewRole { get; }
        public UpdateUserRoleEvent(Guid userId, string newRole)
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
