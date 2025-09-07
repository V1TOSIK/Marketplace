namespace ProductModule.Application.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string MainMediaUrl { get; set; } = string.Empty;
        public IEnumerable<string>? MediaUrls { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PriceAmount { get; set; }
        public string PriceCurrency { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
