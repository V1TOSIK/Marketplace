namespace ProductModule.Application.Dtos.Requests
{
    public class ProductFilterRequest
    {
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public List<string>? Location { get; set; } = [];
        public int? CategoryId { get; set; }
        public List<CharacteristicFilterRequest>? Characteristics { get; set; } = [];
    }
}
