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

        private readonly List<UserPhoneNumber> _phoneNumbers = [];
        public IReadOnlyCollection<UserPhoneNumber> PhoneNumbers => _phoneNumbers.AsReadOnly();

        private readonly List<UserBlock> _blocks = [];
        public IReadOnlyCollection<UserBlock> Blocks => _blocks.AsReadOnly();

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

        public IEnumerable<UserPhoneNumber> GetPhoneNumbers()
        {
            if (IsDeleted)
                throw new InvalidUserOperationException("Cannot retrieve phone numbers of a deleted user.");
            return _phoneNumbers;
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

        public void BlockUser(Guid blockedUserId)
        {
            if (blockedUserId == Guid.Empty)
                throw new InvalidBlockDataException("Blocked user ID cannot be empty");
            if (Blocks.Any(b => (b.BlockedUserId == blockedUserId && b.UnblockedAt == null)))
                throw new BlockExistException("User is already blocked");
            var block = UserBlock.Create(Id, blockedUserId);
            _blocks.Add(block);
        }
        public void UnblockUser(Guid blockedUserId)
        {
            var block = _blocks.FirstOrDefault(b => b.BlockedUserId == blockedUserId);
            if (block == null)
                throw new BlockNotFoundException("Block not found");
            block.Unblock();
        }

        public void MarkAsDeleted()
        {
            if (IsDeleted)
                throw new DeleteUserException("User is already deleted.");
            IsDeleted = true;
        }
    }
}
