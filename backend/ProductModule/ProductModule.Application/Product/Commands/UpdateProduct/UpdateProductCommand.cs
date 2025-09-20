using MediatR;
using ProductModule.Application.Dtos;

namespace ProductModule.Application.Product.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest
    {
        public UpdateProductCommand(Guid productId, UpdateProductRequest request)
        {
            ProductId = productId;
            Name = request.Name;
            PriceAmount = request.PriceAmount;
            PriceCurrency = request.PriceCurrency;
            Location = request.Location;
            Description = request.Description;
            CategoryId = request.CategoryId;
            Characteristics = request.Characteristics;
            Groups = request.Groups;
        }
        public Guid ProductId { get; set; }
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
