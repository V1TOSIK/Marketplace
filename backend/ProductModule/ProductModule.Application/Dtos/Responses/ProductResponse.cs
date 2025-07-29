namespace ProductModule.Application.Dtos.Responses
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string MainMediaUrl { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal PriceAmount { get; set; }
        public string PriceCurrency { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Guid UserId { get; set; }
    }
}
