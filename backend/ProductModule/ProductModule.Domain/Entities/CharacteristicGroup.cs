using ProductModule.Domain.Exceptions;
using SharedKernel.Entity;

namespace ProductModule.Domain.Entities
{
    public class CharacteristicGroup : Entity<int>
    {
        public CharacteristicGroup(string name, Guid productId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidCharacteristicTemplateDataException("Characteristic group name cannot be empty or null.");
            if (productId == Guid.Empty)
                throw new InvalidCharacteristicTemplateDataException("Product ID cannot be empty.");

            Name = name;
            ProductId = productId;
        }

        public string Name { get; private set; } = string.Empty;
        public Guid ProductId { get; private set; }


        private readonly List<CharacteristicValue> _characteristicValues = new ();
        public IReadOnlyCollection<CharacteristicValue> CharacteristicValues => _characteristicValues.AsReadOnly();

        public static CharacteristicGroup Create(string name, Guid productId)
        {
            return new CharacteristicGroup(name, productId);
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidCharacteristicGroupDataException("Characteristic group name cannot be empty or null.");
            Name = name;
        }

        public void AddCharacteristic(string value, int templateId)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidCharacteristicGroupDataException("Characteristic value cannot be empty or null.");
            if (templateId <= 0)
                throw new InvalidCharacteristicGroupDataException("Template ID must be a positive integer.");
            if (_characteristicValues.Any(c => c.CharacteristicTemplateId == templateId))
                throw new InvalidCharacteristicGroupDataException("Characteristic with this template ID already exists in the group.");

            var characteristicTemplate = CharacteristicValue.Create(value, templateId);
            _characteristicValues.Add(characteristicTemplate);
        }

        public void RemoveCharacteristic(int charId)
        {
            var characteristic = _characteristicValues.FirstOrDefault(c => c.Id == charId);
            if (characteristic == null)
                throw new InvalidCharacteristicValueDataException("Characteristic not found.");
            _characteristicValues.Remove(characteristic);
        }
    }
}
