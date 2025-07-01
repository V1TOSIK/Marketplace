using SharedKernel.Exceptions;
using SharedKernel.ValueObjects;

namespace UserModule.Domain.Entities
{
    public class UserPhoneNumber
    {
        private UserPhoneNumber(Guid userId, PhoneNumber phoneNumber)
        {
            UserId = userId;
            PhoneNumber = phoneNumber;
        }
        public int Id { get; private set; }
        public Guid UserId { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }

        public static UserPhoneNumber Create(Guid userId, string phoneNumberValue)
        {
            if (userId == Guid.Empty)
                throw new InvalidPhoneNumberFormatException("User ID cannot be empty");
            if (string.IsNullOrWhiteSpace(phoneNumberValue))
                throw new InvalidPhoneNumberFormatException("Phone number cannot be empty or null");
            var phoneNumber = new PhoneNumber(phoneNumberValue);
            return new UserPhoneNumber(userId, phoneNumber);
        }
    }
}
