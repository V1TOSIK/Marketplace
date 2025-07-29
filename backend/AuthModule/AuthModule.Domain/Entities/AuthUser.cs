using AuthModule.Domain.Enums;
using AuthModule.Domain.Exceptions;
using AuthModule.Domain.ValueObjects;
using SharedKernel.Exceptions;
using SharedKernel.ValueObjects;

namespace AuthModule.Domain.Entities
{
    public class AuthUser
    {
        private AuthUser(Guid id,
            Email? email,
            PhoneNumber? phoneNumber,
            Password password,
            UserRole role)
        {
            if (email is null && phoneNumber is null)
                throw new MissingAuthCredentialException();

            Id = id;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Role = role;
            RegistrationDate = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }
        public Email? Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public Password Password { get; private set; }
        public UserRole Role { get; private set; } = UserRole.Guest;
        public DateTime RegistrationDate { get; }
        public bool IsBanned { get; private set; } = false;
        public DateTime? BannedAt { get; private set; } = null;
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

            var role = ParseRole(roleText);

            return new AuthUser(Guid.NewGuid(), email, phoneNumber, password, role);
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

        public void Ban()
        {
            if (IsBanned)
                throw new UserOperationException("User is already baned.");
            IsBanned = true;
            BannedAt = DateTime.UtcNow;
        }

        public void Unban()
        {
            if (!IsBanned)
                throw new UserOperationException("User is not baned.");
            IsBanned = false;
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
            Password = new Password(passwordValue);
        }
        public void UpdateRole(string roleText)
        {
            Role = ParseRole(roleText);
        }
        public bool ThrowIfCannotLogin()
        {
            if (IsDeleted)
                throw new UserOperationException("User is deleted.");
            if (IsBanned)
                throw new UserOperationException("User is baned.");
            return true;
        }
        public bool CanLogin()
        {
            return !IsDeleted && !IsBanned;
        }
        private static UserRole ParseRole(string roleText)
        {
            if (!Enum.TryParse<UserRole>(roleText, true, out var parsedRole))
                throw new InvalidUserRoleException($"Invalid user role: {roleText}");
            return parsedRole;
        }
    }
}
