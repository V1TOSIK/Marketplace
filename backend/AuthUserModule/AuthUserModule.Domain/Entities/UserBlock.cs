namespace AuthUserModule.Domain.Entities
{
    public class UserBlock
    {
        private UserBlock() { }

        public UserBlock(Guid userId, Guid blockByUserId, string reason)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            if (blockByUserId == Guid.Empty)
                throw new ArgumentException("Blocking User ID cannot be empty.", nameof(blockByUserId));
            if (userId == blockByUserId)
                throw new ArgumentException("User cannot block themselves.", nameof(userId));

            UserId = userId;
            BlockedByUserId = blockByUserId;
            Reason = reason ?? string.Empty;
            BlockedAt = DateTime.UtcNow;
        }
        public int BlockId { get; private set; }
        public Guid UserId { get;}
        public Guid BlockedByUserId { get;}
        public string Reason { get; } = string.Empty;
        public DateTime BlockedAt { get;} = DateTime.UtcNow;

    }
}
