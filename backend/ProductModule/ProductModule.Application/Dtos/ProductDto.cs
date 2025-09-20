using SharedKernel.Dtos;

namespace ProductModule.Application.Dtos
{
    public class ProductDto
    {
        public ProductDto(Guid id,
            Guid userId,
            IEnumerable<MediaDto>? medias,
            string name,
            decimal priceAmount,
            string priceCurrency,
            string location,
            string? description,
            int categoryId,
            string status)
        {
            Id = id;
            UserId = userId;
            Medias = medias;
            Name = name;
            PriceAmount = priceAmount;
            PriceCurrency = priceCurrency;
            Location = location;
            Description = description;
            CategoryId = categoryId;
            Status = status;
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public IEnumerable<MediaDto>? Medias { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PriceAmount { get; set; }
        public string PriceCurrency { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
