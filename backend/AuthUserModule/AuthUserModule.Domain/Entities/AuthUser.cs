using AuthUserModule.Domain.Enums;
using AuthUserModule.Domain.Exceptions;
using AuthUserModule.Domain.ValueObjects;

namespace AuthUserModule.Domain.Entities
{
    internal class AuthUser
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

        private List<UserBlock> _blocks = new();
        public IReadOnlyCollection<UserBlock> Blocks => _blocks.AsReadOnly();
        
        public static AuthUser Create(string? emailValue, string? phoneNumberValue, string passwordValue, UserRole role)
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

            return new AuthUser(Guid.NewGuid(), email, phoneNumber, password, role);
        }

        public void AddBlock(UserBlock block)
        {
            if (block is null)
                throw new ArgumentNullException(nameof(block));

            _blocks.Add(block);
        }

        public void Unblock()
        {
            _blocks.Clear();
        }

        public bool IsBlocked() => _blocks.Any();
    }
}
