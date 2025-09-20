using AuthModule.Domain.Enums;

namespace AuthModule.Domain.Entities
{
    public class ExternalLogin
    {
        private ExternalLogin(Guid userId, AuthProvider provider, string providerUserId)
        {
            UserId = userId;
            Provider = provider;
            ProviderUserId = providerUserId;
        }

        public Guid UserId { get; private set; }
        public AuthProvider Provider { get; private set; }
        public string ProviderUserId { get; private set; } = string.Empty;

        public static ExternalLogin Create(Guid userId, AuthProvider provider, string providerUserId)
        {
            if (string.IsNullOrWhiteSpace(providerUserId))
                throw new ArgumentException("Provider user id cannot be null or empty", nameof(providerUserId));

            return new ExternalLogin(userId, provider, providerUserId);
        }
    }
}
