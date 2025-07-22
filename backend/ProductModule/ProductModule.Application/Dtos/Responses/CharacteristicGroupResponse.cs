namespace ProductModule.Application.Dtos.Responses
{
    public class CharacteristicGroupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CharacteristicResponse> Characteristics { get; set; } = new List<CharacteristicResponse>();
    }
}
