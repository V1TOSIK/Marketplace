using ProductModule.Domain.Exceptions;
using SharedKernel.Entity;

namespace ProductModule.Domain.Entities
{
    public class CharacteristicValue : Entity<int>
    {
        private CharacteristicValue(string value, int characteristicTemplateId)
        {
            Value = value;
            CharacteristicTemplateId = characteristicTemplateId;
        }

        public string Value { get; private set; } = string.Empty;
        public int CharacteristicTemplateId { get; private set; }
        public int GroupId { get; set; }

        public static CharacteristicValue Create(string value, int characteristicTemplateId)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidCharacteristicValueDataException("Characteristic value cannot be empty or null.");
            if (characteristicTemplateId <= 0)
                throw new InvalidCharacteristicValueDataException("Characteristic template ID must be a positive integer.");

            return new CharacteristicValue(value, characteristicTemplateId);
        }
        public void UpdateValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidCharacteristicValueDataException("Value cannot be empty.");
            Value = value;
        }
    }
}
