namespace ProductModule.Application.Dtos.Responses
{
    public class CharacteristicGroupResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<CharacteristicResponse> Characteristics { get; set; } = new List<CharacteristicResponse>();
    }
}
