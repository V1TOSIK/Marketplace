using AuthUserModule.Domain.Exceptions;
using System.Security.Cryptography;

namespace AuthUserModule.Domain.Entities
{
    public class RefreshToken
    {
        private RefreshToken(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new RefreshTokenException("User ID cannot be empty.");
            Id = Guid.NewGuid();
            UserId = userId;
            ExpirationDate = DateTime.UtcNow.AddDays(30);
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Token { get; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        public DateTime ExpirationDate { get; private set; }
        public bool IsRevoked { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; private set; }

        public static RefreshToken Create(Guid userId)
        {
            return new RefreshToken(userId);
        }

        public void Revoke()
        {
            if (IsRevoked)
                throw new InvalidOperationException("Token is already revoked.");

            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
        }

        public void UpdateTokenExpiration(DateTime newExpirationDate)
        {
            if (newExpirationDate <= DateTime.UtcNow)
                throw new RefreshTokenException("New expiration date must be in the future.");
            ExpirationDate = newExpirationDate;
        }
    }
}
