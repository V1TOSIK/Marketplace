using UserModule.Domain.Exceptions;

namespace UserModule.Domain.Entities
{
    public class User
    {
        private User(Guid id, string name, string location)
        {
            Id = id;
            Name = name;
            Location = location;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;
        public bool IsDeleted { get; private set; } = false;
        public bool IsBanned { get; private set; } = false;

        private readonly List<UserPhoneNumber> _phoneNumbers = [];
        public IReadOnlyCollection<UserPhoneNumber> PhoneNumbers => _phoneNumbers.AsReadOnly();

        public static User Create(Guid id, string name, string location)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidUserDataException("Name cannot be empty or null");

            return new User(id, name, location);
        }

        public void UpdateName(string name)
        {
            if (IsDeleted)
                throw new InvalidUserOperationException("Cannot update a deleted user.");

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidUserDataException("Name cannot be empty or null");
            Name = name;
        }

        public void UpdateLocation(string location)
        {
            if (IsDeleted)
                throw new InvalidUserOperationException("Cannot update a deleted user.");

            if (string.IsNullOrWhiteSpace(location))
                throw new InvalidUserDataException("Location cannot be empty or null");
            Location = location;
        }

        public IEnumerable<UserPhoneNumber> GetPhoneNumbers()
        {
            if (IsDeleted)
                throw new InvalidUserOperationException("Cannot retrieve phone numbers of a deleted user.");
            return _phoneNumbers;
        }

        public IEnumerable<string> GetPhoneNumbersValue()
        {
            if (IsDeleted)
                throw new InvalidUserOperationException("Cannot retrieve phone numbers of a deleted user.");
            return _phoneNumbers.Select(pn => pn.PhoneNumber.Value);
        }

        public void AddPhoneNumber(string phoneNumberValue)
        {
            if (IsDeleted)
                throw new InvalidUserOperationException("Cannot update a deleted user.");

            if (_phoneNumbers.Any(p => p.PhoneNumber.Value == phoneNumberValue))
                throw new PhoneNumberIsAlreadyAddedException("This phone number is already added.");
            if (string.IsNullOrWhiteSpace(phoneNumberValue))
                throw new NullablePhoneNumberException("Phone number cannot be empty or null");
            var phoneNumber = UserPhoneNumber.Create(Id, phoneNumberValue);
            _phoneNumbers.Add(phoneNumber);
        }

        public void RemovePhoneNumber(int phoneNumberId)
        {
            if (IsDeleted)
                throw new InvalidUserOperationException("Cannot update a deleted user.");

            var phoneNumber = PhoneNumbers.FirstOrDefault(p => p.Id == phoneNumberId);
            if (phoneNumber == null)
                throw new PhoneNumberNotFoundException("Phone number not found");
            _phoneNumbers.Remove(phoneNumber);
        }
        public void MarkAsDeleted()
        {
            if (IsDeleted)
                throw new DeleteUserException("User is already deleted.");
            IsDeleted = true;
        }
        public void Restore()
        {
            if (!IsDeleted)
                throw new RestoreUserException("User is not deleted.");
            IsDeleted = false;
        }

        public void Ban()
        {
            if(!IsBanned)
                throw new BannedUserException("User is already banned.");
            IsBanned = true;
        }
    }
}
