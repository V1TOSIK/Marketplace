using ProductModule.Domain.Exceptions;
using SharedKernel.AgregateRoot;
using SharedKernel.Events;

namespace ProductModule.Domain.Entities
{
    public class Category : AggregateRoot<int>
    {
        private Category(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidCategoryDataException("Category name cannot be empty or null.");
            Name = name;
        }
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

        public void Delete()
        {
            AddDomainEvent(new DeleteCategoryEvent(Id));
        }
    }
}
