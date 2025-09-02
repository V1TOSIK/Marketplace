namespace ProductModule.Application.Dtos
{
    public class CharacteristicFilter
    {
        public int TemplateId { get; set; }
        public IEnumerable<string> Values { get; set; } = [];
    }
}
