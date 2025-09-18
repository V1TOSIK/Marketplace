namespace ProductModule.Application.Dtos
{
    public class CharacteristicCreateDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = default!;
        public string Value { get; set; } = default!;
        public string? Unit { get; set; }
    }
}
