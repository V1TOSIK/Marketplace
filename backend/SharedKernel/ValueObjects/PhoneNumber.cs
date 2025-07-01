using SharedKernel.Exceptions;
using System.Text.RegularExpressions;

namespace SharedKernel.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        public string Value { get; }

        private static readonly Regex phoneNumberRegex = new(
            @"^\+?\d{10,15}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private PhoneNumber() { }

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidPhoneNumberFormatException("Phone number cannot be empty or null");

            if (!phoneNumberRegex.IsMatch(value))
                throw new InvalidPhoneNumberFormatException("Phone number format is invalid");

            Value = value;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;

        public static explicit operator PhoneNumber(string value) => new(value);
    }
}
