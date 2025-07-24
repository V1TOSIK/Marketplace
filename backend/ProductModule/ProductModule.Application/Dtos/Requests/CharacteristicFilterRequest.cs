namespace ProductModule.Application.Dtos.Requests
{
    public class CharacteristicFilterRequest
    {
        public int TemplateId { get; set; }
        public List<string> Values { get; set; } = [];
    }
}
