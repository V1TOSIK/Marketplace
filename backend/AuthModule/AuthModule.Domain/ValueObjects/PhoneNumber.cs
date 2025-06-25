using AuthModule.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace AuthModule.Domain.ValueObjects
{
    public record PhoneNumber
    {
        public string Value { get; private set; }

        private static readonly Regex phoneNumberRegex = new(
            @"^\+?\d{10,15}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private PhoneNumber() { }

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidPhoneNumberFormatException("Phone number cannot be empty or null");

            if (!phoneNumberRegex.IsMatch(value))
                throw new InvalidPhoneNumberFormatException();

            Value = value;
        }

        public override string ToString() => Value;
    }
}
