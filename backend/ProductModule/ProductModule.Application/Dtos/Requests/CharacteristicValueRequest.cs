namespace ProductModule.Application.Dtos.Requests
{
    public class CharacteristicValueRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }
}
