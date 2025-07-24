using ProductModule.Domain.Enums;
using ProductModule.Domain.Exceptions;

namespace ProductModule.Domain.Entities
{
    public class CharacteristicTemplate
    {
        private CharacteristicTemplate(string name, string unit, int categoryId, TemplateType type)
        {
            Name = name;
            Unit = unit;
            CategoryId = categoryId;
            Type = type;
        }
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Unit { get; private set; } = string.Empty;
        public int CategoryId { get; private set; }
        public TemplateType Type { get; private set; }

        public static CharacteristicTemplate Create(string name, string? unit, int categoryId, string templateTypeValue)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidCharacteristicTemplateDataException("Characteristic template name cannot be empty or null.");
            unit = unit ?? string.Empty;

            if (categoryId <= 0)
                throw new InvalidCharacteristicTemplateDataException("Category ID must be a positive integer.");

            if (!Enum.TryParse<TemplateType>(templateTypeValue, out var templateType))
                throw new InvalidCharacteristicTemplateDataException("Invalid template type");

            return new CharacteristicTemplate(name, unit, categoryId, templateType);
        }
    }
}
