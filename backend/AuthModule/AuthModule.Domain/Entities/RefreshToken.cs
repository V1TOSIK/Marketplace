using AuthModule.Domain.Exceptions;
using System.Security.Cryptography;

namespace AuthModule.Domain.Entities
{
    public class RefreshToken
    {
        public const byte EXPIRATION_DAYS = 10;
        private RefreshToken(Guid userId, Guid? replacedByTokenId)
        {
            if (userId == Guid.Empty)
                throw new InvalidRefreshTokenException("User ID cannot be empty.");
            Id = Guid.NewGuid();
            UserId = userId;
            ExpirationDate = DateTime.UtcNow.AddDays(EXPIRATION_DAYS);
            ReplacedByTokenId = replacedByTokenId;

            Token = GenerateToken();
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Token { get; }
        public DateTime ExpirationDate { get; private set; }
        public bool IsRevoked { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; private set; }

        public Guid? ReplacedByTokenId { get; set; }

        public static RefreshToken Create(Guid userId, Guid? replacedByTokenId = null)
        {
            return new RefreshToken(userId, replacedByTokenId);
        }

        public void Revoke()
        {
            if (IsRevoked)
                throw new RefreshTokenOperationException("Token is already revoked.");

            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
        }

        public void UpdateTokenExpiration(DateTime newExpirationDate)
        {
            if (newExpirationDate <= DateTime.UtcNow)
                throw new RefreshTokenOperationException("New expiration date must be in the future.");
            ExpirationDate = newExpirationDate;
        }

        private string GenerateToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
