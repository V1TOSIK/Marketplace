using AuthModule.Domain.Exceptions;
using SharedKernel.ValueObjects;
using System.Text.RegularExpressions;

namespace AuthModule.Domain.ValueObjects
{
    public sealed class Email : ValueObject
    {
        public string Value { get; }

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

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(Email email) => email.Value;
        public static explicit operator Email(string value) => new Email(value);
    }
}