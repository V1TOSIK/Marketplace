using AuthModule.Domain.Exceptions;

namespace AuthModule.Domain.ValueObjects
{
    public record Password
    {
        public string Value { get; private set; }

        private Password() { }

        public Password(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidPasswordFormatException("Password cannot be empty or null");

            Value = value;
        }

        public override string ToString() => Value;
    }
}
