using ProductModule.Domain.Exceptions;
using ProductModule.Domain.ValueObjects;

namespace ProductModule.Domain.Entities
{
    public class Product
    {
        private Product() { }
        private Product(Guid userId,
            string name,
            Price price,
            string location,
            string description,
            int categoryId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            Price = price;
            Location = location;
            Description = description;
            CategoryId = categoryId;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Price Price { get; private set; }
        public string Location { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        public int CategoryId { get; private set; }

        private readonly List<CharacteristicGroup> _characteristicGroups = new ();
        public IReadOnlyCollection<CharacteristicGroup> CharacteristicGroups => _characteristicGroups.AsReadOnly();

        public static Product Create(Guid userId,
            string name,
            string priceCurrency,
            decimal priceAmount,
            string location,
            string description,
            int categoryId)
        {
            var price = new Price(priceAmount,  priceCurrency);

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidProductDataException("Product name cannot be empty or null.");
            if (price == null)
                throw new InvalidProductDataException("Price cannot be null.");
            if (string.IsNullOrWhiteSpace(location))
                throw new InvalidProductDataException("Location cannot be empty or null.");
            if (string.IsNullOrWhiteSpace(description))
                throw new InvalidProductDataException("Description cannot be empty or null.");

            return new Product(userId, name, price, location, description, categoryId);
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidProductDataException("Product name cannot be empty or null.");
            Name = name;
        }

        public void UpdatePrice(string priceValue)
        {
            var priceParts = priceValue.Split(' ');
            if (priceParts.Length != 2 || !decimal.TryParse(priceParts[0], out var amount) || string.IsNullOrWhiteSpace(priceParts[1]))
            {
                throw new InvalidProductDataException("Invalid price format. Expected format: 'amount currency'.");
            }
            Price = new Price(amount, priceParts[1]);
        }

        public void UpdateLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new InvalidProductDataException("Location cannot be empty or null.");
            Location = location;
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new InvalidProductDataException("Description cannot be empty or null.");
            Description = description;
        }

        public void UpdateCategory(int categoryId)
        {
            if (categoryId <= 0)
                throw new InvalidProductDataException("Category ID must be a positive integer.");
            CategoryId = categoryId;
        }

        public void AddCharacteristicGroup(CharacteristicGroup characteristicGroup)
        {
            if (characteristicGroup == null)
                throw new InvalidCharacteristicGroupDataException("Characteristic group cannot be null.");

            _characteristicGroups.Add(characteristicGroup);
        }
    }
}
