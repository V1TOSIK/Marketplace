using MediatR;
using SharedKernel.AgregateRoot;
using SharedKernel.Events;
using UserModule.Domain.Exceptions;

namespace UserModule.Domain.Entities
{
    public class User : AggregateRoot<Guid>
    {
        private User(Guid id, string name, string location)
        {
            Id = id;
            Name = name;
            Location = location;
        }

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

        public void UpdateUser(string? name, string? location, IEnumerable<string>? newPhones)
        {
            EnsureActive();

            if (!string.IsNullOrWhiteSpace(name))
                UpdateName(name);

            if (!string.IsNullOrWhiteSpace(location))
                UpdateLocation(location);

            if (newPhones != null)
            {
                var phonesToRemove = _phoneNumbers
                    .Where(p => !newPhones.Any(np => string.Equals(np?.Trim(), p.PhoneNumber.Value, StringComparison.OrdinalIgnoreCase)))
                    .Select(p => p.PhoneNumber.Value)
                    .ToList();

                foreach (var phone in phonesToRemove)
                    RemovePhoneNumber(phone);

                foreach (var phone in newPhones.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    if (!_phoneNumbers.Any(p => p.PhoneNumber.Value == phone))
                        AddPhoneNumber(phone);
                }
            }
        }

        private void UpdateName(string name)
        {
            EnsureActive();

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidUserDataException("Name cannot be empty or null");
            Name = name;
        }

        private void UpdateLocation(string location)
        {
            EnsureActive();

            if (string.IsNullOrWhiteSpace(location))
                throw new InvalidUserDataException("Location cannot be empty or null");
            Location = location;
        }

        public IEnumerable<string> GetPhoneNumbersValue()
        {
            EnsureActive();
            return _phoneNumbers.Select(pn => pn.PhoneNumber.Value).ToList();
        }

        private void EnsureActive()
        {
            if (IsDeleted)
                throw new InvalidUserOperationException("Cannot validate a deleted user.");
            if (IsBanned)
                throw new InvalidUserOperationException("Cannot validate a banned user.");
        }

        public void AddPhoneNumber(string phoneNumberValue)
        {
            EnsureActive();

            if (_phoneNumbers.Any(p => p.PhoneNumber.Value == phoneNumberValue))
                throw new PhoneNumberIsAlreadyAddedException("This phone number is already added.");
            if (string.IsNullOrWhiteSpace(phoneNumberValue))
                throw new NullablePhoneNumberException("Phone number cannot be empty or null");
            var phoneNumber = UserPhoneNumber.Create(Id, phoneNumberValue);
            _phoneNumbers.Add(phoneNumber);
        }

        public void RemovePhoneNumber(string phoneNumberValue)
        {
            EnsureActive();

            var phoneNumber = PhoneNumbers.FirstOrDefault(p => p.PhoneNumber.Value == phoneNumberValue);
            if (phoneNumber == null)
                throw new PhoneNumberNotFoundException("Phone number not found");
            _phoneNumbers.Remove(phoneNumber);
        }

        public void MarkAsDeleted()
        {
            if (IsDeleted)
                throw new DeleteUserException("User is already deleted.");
            IsDeleted = true;
            AddDomainEvent(new SoftDeleteUserDomainEvent(Id));
        }

        public void Delete()
        {
            AddDomainEvent(new HardDeleteUserDomainEvent(Id));
        }

        public void Restore()
        {
            if (!IsDeleted)
                throw new RestoreUserException("User is not deleted.");
            IsDeleted = false;
        }

        public void Ban(string reason)
        {
            if (IsBanned)
                throw new BanUserException("User is already banned.");
            IsBanned = true;
            AddDomainEvent(new BanUserDomainEvent(Id, reason));
        }

        public void UnBan()
        {
            if (!IsBanned)
                throw new UnbanUserException("User is not banned.");
            IsBanned = false;
            AddDomainEvent(new UnbanUserDomainEvent(Id));
        }
    }
}
