using AuthModule.Domain.Enums;
using AuthModule.Domain.Exceptions;
using AuthModule.Domain.ValueObjects;
using SharedKernel.AgregateRoot;
using SharedKernel.Events;
using SharedKernel.Exceptions;
using SharedKernel.ValueObjects;

namespace AuthModule.Domain.Entities
{
    public class AuthUser : AggregateRoot<Guid>
    {
        private AuthUser(
            string? providerUserId,
            Email? email,
            PhoneNumber? phoneNumber,
            Password? password,
            AuthProvider provider,
            UserRole role)
        {
            if (email is null && phoneNumber is null)
                throw new MissingAuthCredentialException();

            ProviderUserId = providerUserId;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = provider == AuthProvider.Local
                ? password ?? throw new InvalidPasswordFormatException("Password can not be null")
                : null;
            Provider = provider;
            Role = role;
            RegistrationDate = DateTime.UtcNow;
        }

        public string? ProviderUserId { get; private set; }
        public Email? Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public Password? Password { get; private set; }
        public AuthProvider Provider { get; private set; } = AuthProvider.Local;
        public UserRole Role { get; private set; } = UserRole.Guest;
        public DateTime RegistrationDate { get; }
        public bool IsBanned { get; private set; } = false;
        public string? BanReason { get; private set; } = null;
        public DateTime? BannedAt { get; private set; } = null;
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAt { get; private set; }

        public static AuthUser CreateOAuth(string providerUserId, string? email, string providerText)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new MissingAuthCredentialException("Email is required for OAuth users.");

            if (string.IsNullOrWhiteSpace(providerUserId))
                throw new InvalidProviderException("Provider user id cannot be null or empty");

            var provider = ParseProvider(providerText);

            return new AuthUser(
                providerUserId,
                new Email(email),
                null,
                null,
                provider,
                UserRole.User
            );
        }

        public static AuthUser Create(string? emailValue, string? phoneNumberValue, string passwordValue)
        {
            var email = string.IsNullOrWhiteSpace(emailValue) ? null : new Email(emailValue);
            var phoneNumber = string.IsNullOrWhiteSpace(phoneNumberValue) ? null : new PhoneNumber(phoneNumberValue);

            var password = new Password(passwordValue);

            return new AuthUser(
                null,
                email,
                phoneNumber,
                password,
                AuthProvider.Local,
                UserRole.User);
        }

        public void MarkAsDeleted()
        {
            if (IsDeleted)
                throw new UserOperationException("User is already deleted.");

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (!IsDeleted)
                throw new UserOperationException("User is not deleted.");

            IsDeleted = false;
            DeletedAt = null;
            AddDomainEvent(new RestoreUserDomainEvent(Id));
        }

        public void Ban(string reason)
        {
            if (IsBanned)
                throw new UserOperationException("User is already baned.");
            IsBanned = true;
            BanReason = reason;
            BannedAt = DateTime.UtcNow;
        }

        public void Unban()
        {
            if (!IsBanned)
                throw new UserOperationException("User is not baned.");
            IsBanned = false;
            BanReason = null;
            BannedAt = null;
        }

        public void AddEmail(string emailValue)
        {
            if (string.IsNullOrWhiteSpace(emailValue))
                throw new InvalidEmailFormatException("Email cannot be empty or null.");
            if (Email != null)
                throw new UserOperationException("Email is already set. Use UpdateEmail method to change it.");
            Email = new Email(emailValue);
        }

        public void AddPhone(string phoneValue)
        {
            if (string.IsNullOrWhiteSpace(phoneValue))
                throw new InvalidPhoneNumberFormatException("Phone number cannot be empty or null.");
            if (PhoneNumber != null)
                throw new UserOperationException("Phone number is already set. Use UpdatePhoneNumber method to change it.");
            PhoneNumber = new PhoneNumber(phoneValue);
        }

        public void UpdatePassword(string passwordValue)
        {
            if (string.IsNullOrWhiteSpace(passwordValue))
                throw new InvalidPasswordFormatException("Password cannot be empty or null.");
            if (IsOAuth())
                throw new UserOperationException("User is registered by OAuth");
            Password = new Password(passwordValue);
        }

        public void UpdateRole(string roleText) => Role = ParseRole(roleText);

        public bool ThrowIfCannotLogin()
        {
            if (IsDeleted)
                throw new UserOperationException("User is deleted.");
            if (IsBanned)
                throw new UserOperationException("User is baned.");
            return true;
        }

        public bool CanLogin() => !IsDeleted && !IsBanned;

        public bool IsOAuth() => Provider != AuthProvider.Local;

        public bool IsLocal() => Provider == AuthProvider.Local;

        private static AuthProvider ParseProvider(string providerText)
        {
            if (!Enum.TryParse<AuthProvider>(providerText, true, out var parsedProvider))
                throw new InvalidProviderException($"Invalid auth provider: {providerText}");
            return parsedProvider;
        }

        private static UserRole ParseRole(string roleText)
        {
            if (!Enum.TryParse<UserRole>(roleText, true, out var parsedRole))
                throw new InvalidUserRoleException($"Invalid user role: {roleText}");
            return parsedRole;
        }
    }
}
