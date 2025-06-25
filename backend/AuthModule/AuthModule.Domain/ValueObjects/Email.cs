using AuthModule.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace AuthModule.Domain.ValueObjects
{
    public record Email
    {
        public string Value { get; private set; }

        private static readonly Regex emailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Email() { }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidEmailFormatException("Email cannot be empty or null");

            if (!emailRegex.IsMatch(value))
                throw new InvalidEmailFormatException();

            Value = value;
        }

        public override string ToString() => Value;
    }
}
