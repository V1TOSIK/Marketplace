using UserModule.Domain.Exceptions;

namespace UserModule.Domain.Entities
{
    public class UserBlock
    {
        private UserBlock(Guid userId, Guid blockedUserId)
        {
            UserId = userId;
            BlockedUserId = blockedUserId;
            BlockedAt = DateTime.UtcNow;
        }

        public int Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid BlockedUserId { get; private set; }
        public DateTime BlockedAt { get; private set; }
        public DateTime? UnblockedAt { get; private set; } = null;

        public static UserBlock Create(Guid userId, Guid blockedUserId)
        {
            if (userId == Guid.Empty)
                throw new InvalidBlockDataException("User ID cannot be empty");
            if (blockedUserId == Guid.Empty)
                throw new InvalidBlockDataException("Blocked user ID cannot be empty");
            if (userId == blockedUserId)
                throw new InvalidBlockDataException("You cannot block yourself");

            return new UserBlock(userId, blockedUserId);
        }

        public void Unblock()
        {
            if (UnblockedAt.HasValue)
                throw new BlockExistException("User is already unblocked.");
            UnblockedAt = DateTime.UtcNow;
        }
    }
}
