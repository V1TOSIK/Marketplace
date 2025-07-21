using ProductModule.Domain.Exceptions;

namespace ProductModule.Domain.Entities
{
    public class CharacteristicValue
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
    }
}
