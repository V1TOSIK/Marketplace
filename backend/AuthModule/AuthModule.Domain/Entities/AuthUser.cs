using AuthModule.Domain.Enums;
using AuthModule.Domain.Exceptions;
using AuthModule.Domain.ValueObjects;
using SharedKernel.Exceptions;
using SharedKernel.ValueObjects;

namespace AuthModule.Domain.Entities
{
    public class AuthUser
    {
        private AuthUser(Guid userId,
            Email? email,
            PhoneNumber? phoneNumber,
            Password password,
            UserRole role)
        {
            if (email is null && phoneNumber is null)
                throw new MissingAuthCredentialException();

            UserId = userId;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Role = role;
            RegistrationDate = DateTime.UtcNow;
        }

        public Guid UserId { get; private set; }
        public Email? Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public Password Password { get; private set; }
        public UserRole Role { get; private set; } = UserRole.Guest;
        public DateTime RegistrationDate { get; }
        public bool IsBlocked { get; private set; } = false;
        public DateTime? BlockedAt { get; private set; } = null;
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAt { get; private set; }

        public static AuthUser Create(string? emailValue, string? phoneNumberValue, string passwordValue, string roleText)
        {
            if (string.IsNullOrWhiteSpace(emailValue) && string.IsNullOrWhiteSpace(phoneNumberValue))
                throw new MissingAuthCredentialException();

            var email = string.IsNullOrWhiteSpace(emailValue)
                ? null
                : new Email(emailValue);
            var phoneNumber = string.IsNullOrWhiteSpace(phoneNumberValue)
                ? null
                : new PhoneNumber(phoneNumberValue);

            var password = new Password(passwordValue);

            if (!Enum.TryParse<UserRole>(roleText, true, out var parsedRole))
                throw new InvalidUserRoleException($"Invalid user role: {roleText}");

            return new AuthUser(Guid.NewGuid(), email, phoneNumber, password, parsedRole);
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
        }

        public void Block()
        {
            if (IsBlocked)
                throw new UserOperationException("User is already blocked.");
            IsBlocked = true;
            BlockedAt = DateTime.UtcNow;
        }

        public void Unblock()
        {
            if (!IsBlocked)
                throw new UserOperationException("User is not blocked.");
            IsBlocked = false;
            BlockedAt = null;
        }
        public void UpdateEmail(string emailValue)
        {
            if (string.IsNullOrWhiteSpace(emailValue))
                throw new InvalidEmailFormatException("Email cannot be empty or null.");
            Email = new Email(emailValue);
        }
        public void UpdatePhoneNumber(string phoneNumberValue)
        {
            if (string.IsNullOrWhiteSpace(phoneNumberValue))
                throw new InvalidPhoneNumberFormatException("Phone number cannot be empty or null.");
            PhoneNumber = new PhoneNumber(phoneNumberValue);
        }
        public void UpdatePassword(string passwordValue)
        {
            if (string.IsNullOrWhiteSpace(passwordValue))
                throw new InvalidPasswordFormatException("Password cannot be empty or null.");
            Password = new Password(passwordValue);
        }
        public void UpdateRole(string roleText)
        {
            if (!Enum.TryParse<UserRole>(roleText, true, out var parsedRole))
                throw new InvalidUserRoleException($"Invalid user role: {roleText}");
            Role = parsedRole;
        }
    }
}
