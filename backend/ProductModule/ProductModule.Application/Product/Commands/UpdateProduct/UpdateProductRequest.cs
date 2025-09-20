using ProductModule.Application.Dtos;

namespace ProductModule.Application.Product.Commands.UpdateProduct
{
    public class UpdateProductRequest
    {
        public string? Name { get; set; } = string.Empty;
        public decimal? PriceAmount { get; set; }
        public string? PriceCurrency { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int? CategoryId { get; set; }

        public CharacteristicBatchDto Characteristics { get; set; } = new();
        public GroupBatchDto Groups { get; set; } = new();
    }
}
