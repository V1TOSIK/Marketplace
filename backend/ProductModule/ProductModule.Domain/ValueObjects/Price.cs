using SharedKernel.ValueObjects;

namespace ProductModule.Domain.ValueObjects
{
    public class Price : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        private Price() { }
        public Price(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Price amount cannot be negative.");
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));
            Amount = amount;
            Currency = currency;
        }

        public override string ToString() => $"{Amount} {Currency}";

        public static implicit operator string(Price price) => price.ToString();
        public static explicit operator Price(string value)
        {
            var parts = value.Split(' ');
            if (parts.Length != 2 || !decimal.TryParse(parts[0], out var amount) || string.IsNullOrWhiteSpace(parts[1]))
            {
                throw new FormatException("Invalid price format. Expected format: 'amount currency'.");
            }
            return new Price(amount, parts[1]);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}