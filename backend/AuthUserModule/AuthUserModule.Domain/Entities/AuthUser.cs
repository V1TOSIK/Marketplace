using AuthUserModule.Domain.Enums;
using AuthUserModule.Domain.Exceptions;
using AuthUserModule.Domain.ValueObjects;

namespace AuthUserModule.Domain.Entities
{
    public class AuthUser
    {
        private AuthUser() { }
        public AuthUser(Guid userId,
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
                throw new InvalidOperationException("User is already deleted.");

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (!IsDeleted)
                throw new InvalidOperationException("User is not deleted.");

            IsDeleted = false;
            DeletedAt = null;
        }

    }
}
