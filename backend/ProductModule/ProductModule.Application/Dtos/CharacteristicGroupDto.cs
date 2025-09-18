namespace ProductModule.Application.Dtos
{
    public class CharacteristicGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CharacteristicDto> Characteristics { get; set; } = new ();
    }
}
