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
        private AuthUser() { }
        private AuthUser(
            Email? email,
            PhoneNumber? phoneNumber,
            Password? password,
            UserRole role)
        {
            if (email is null && phoneNumber is null)
                throw new MissingAuthCredentialException();

            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Role = role;
            RegistrationDate = DateTime.UtcNow;
            AddDomainEvent(new UserRegisteredEvent(Id));
        }

        public Email? Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public Password? Password { get; private set; }
        public UserRole Role { get; private set; } = UserRole.Guest;
        public DateTime RegistrationDate { get; }
        public bool IsBanned { get; private set; } = false;
        public string? BanReason { get; private set; } = null;
        public DateTime? BannedAt { get; private set; } = null;
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAt { get; private set; }

        private readonly List<ExternalLogin> _externalLogins = [];
        public IReadOnlyCollection<ExternalLogin> ExternalLogins => _externalLogins.AsReadOnly();

        public static AuthUser Create(string? emailValue, string? phoneNumberValue, string? passwordValue)
        {
            var email = string.IsNullOrWhiteSpace(emailValue) ? null : new Email(emailValue);
            var phoneNumber = string.IsNullOrWhiteSpace(phoneNumberValue) ? null : new PhoneNumber(phoneNumberValue);
            var password = string.IsNullOrWhiteSpace(passwordValue) ? null : new Password(passwordValue);

            return new AuthUser(
                email,
                phoneNumber,
                password,
                UserRole.User);
        }

        public void AddExternalLogin(string providerUserId, string providerValue)
        {
            if (!Enum.TryParse<AuthProvider>(providerValue, true, out var provider))
                throw new InvalidProviderException($"Invalid provider: {provider}");
            if (_externalLogins.Any(e => e.Provider == provider && e.ProviderUserId == providerUserId))
                throw new UserOperationException("External login already exists");

            _externalLogins.Add(ExternalLogin.Create(Id, provider, providerUserId));
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
            AddDomainEvent(new RestoreUserEvent(Id));
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

        public void SetEmail(string emailValue)
        {
            EnsureActive();
            if (string.IsNullOrWhiteSpace(emailValue))
                throw new InvalidEmailFormatException("Email cannot be empty or null.");
            if (Email != null)
                throw new UserOperationException("Email is already set.");
            Email = new Email(emailValue);
        }

        public void SetPhone(string phoneValue)
        {
            EnsureActive();
            if (string.IsNullOrWhiteSpace(phoneValue))
                throw new InvalidPhoneNumberFormatException("Phone number cannot be empty or null.");
            if (PhoneNumber != null)
                throw new UserOperationException("Phone number is already set.");
            PhoneNumber = new PhoneNumber(phoneValue);
        }

        public void SetPassword(string passwordValue)
        {
            EnsureActive();
            if (string.IsNullOrWhiteSpace(passwordValue))
                throw new InvalidPasswordFormatException("Password cannot be empty or null.");
            if (Password != null)
                throw new UserOperationException("Password is already set. Use UpdatePassword method to change it.");
            Password = new Password(passwordValue);;
        }

        public void UpdatePassword(string passwordValue)
        {
            EnsureActive();
            if (string.IsNullOrWhiteSpace(passwordValue))
                throw new InvalidPasswordFormatException("Password cannot be empty or null.");
            if (Password == null)
                throw new UserOperationException("User is registered by OAuth");
            Password = new Password(passwordValue);
        }

        public void UpdateRole(string roleText) => Role = ParseRole(roleText);

        public void EnsureActive()
        {
            if (IsDeleted)
                throw new UserOperationException("User is deleted.");
            if (IsBanned)
                throw new UserOperationException("User is baned.");
        }

        private static UserRole ParseRole(string roleText)
        {
            if (!Enum.TryParse<UserRole>(roleText, true, out var parsedRole))
                throw new InvalidUserRoleException($"Invalid user role: {roleText}");
            return parsedRole;
        }
    }
}
