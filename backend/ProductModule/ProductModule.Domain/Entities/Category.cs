using ProductModule.Domain.Exceptions;

namespace ProductModule.Domain.Entities
{
    public class Category
    {
        private Category(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidCategoryDataException("Category name cannot be empty or null.");
            Name = name;
        }
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;


        private readonly List<CharacteristicTemplate> _characteristicTemplates = new List<CharacteristicTemplate>();
        public IReadOnlyCollection<CharacteristicTemplate> CharacteristicTemplates => _characteristicTemplates.AsReadOnly();


        public static Category Create(string name)
        {
            return new Category(name);
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidCategoryDataException("Category name cannot be empty or null.");
            Name = name;
        }
    }
}
