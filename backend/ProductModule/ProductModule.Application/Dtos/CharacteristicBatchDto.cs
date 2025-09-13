namespace ProductModule.Application.Dtos
{
    public class CharacteristicBatchDto
    {
        public List<CharacteristicCreateDto> Added { get; set; } = new();
        public List<CharacteristicUpdateDto> Updated { get; set; } = new();
        public List<int> Deleted { get; set; } = new();
    }
}
