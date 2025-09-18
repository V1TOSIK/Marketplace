using MediatR;
using ProductModule.Application.Dtos;

namespace ProductModule.Application.Characteristic.Queries.GetProductCharacterisitcs
{
    public class GetProductCharacteristicsQuery : IRequest<IEnumerable<CharacteristicGroupDto>>
    {
        public Guid ProductId { get; }
        public GetProductCharacteristicsQuery(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
            ProductId = productId;
        }
    }
}
