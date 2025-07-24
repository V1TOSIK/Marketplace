namespace ProductModule.Application.Dtos.Requests
{
    public class CharacteristicGroupRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<CharacteristicValueRequest> CharacteristicValues { get; set; } = new ();
    }
}
