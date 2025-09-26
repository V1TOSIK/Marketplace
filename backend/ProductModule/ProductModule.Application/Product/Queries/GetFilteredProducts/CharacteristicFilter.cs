namespace ProductModule.Application.Product.Queries.GetFilteredProducts
{
    public class CharacteristicFilter
    {
        public int TemplateId { get; set; }
        public List<string> Values { get; set; } = [];
    }
}
