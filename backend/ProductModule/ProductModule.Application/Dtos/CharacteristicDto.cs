namespace ProductModule.Application.Dtos
{
    public class CharacteristicDto
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Unit { get; set; } = null;
    }
}
