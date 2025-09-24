using ProductModule.Application.Dtos;

namespace ProductModule.Application.Product.Commands.CreateProduct
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal PriceAmount { get; set; }
        public string PriceCurrency { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }

        public List<CharacteristicGroupDto> CharacteristicGroups { get; set; } = new();
    }
}
