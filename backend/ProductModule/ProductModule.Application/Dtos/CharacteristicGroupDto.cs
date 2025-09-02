namespace ProductModule.Application.Dtos
{
    public class CharacteristicGroupDto
    {
        public string Name { get; set; } = string.Empty;
        public List<CharacteristicDto> Characteristics { get; set; } = new ();
    }
}
