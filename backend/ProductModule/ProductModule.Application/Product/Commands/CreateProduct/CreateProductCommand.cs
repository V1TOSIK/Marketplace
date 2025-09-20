using MediatR;
using ProductModule.Application.Dtos;

namespace ProductModule.Application.Product.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public CreateProductCommand(Guid userId, CreateProductRequest request)
        {
            UserId = userId;
            Name = request.Name;
            PriceAmount = request.PriceAmount;
            PriceCurrency = request.PriceCurrency;
            Location = request.Location;
            Description = request.Description;
            CategoryId = request.CategoryId;
            CharacteristicGroups = request.CharacteristicGroups;
        }
        public Guid UserId { get; }
        public string Name { get; set; } = string.Empty;
        public decimal PriceAmount { get; set; }
        public string PriceCurrency { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }

        public List<CharacteristicGroupDto> CharacteristicGroups { get; set; } = new();
    }
}
