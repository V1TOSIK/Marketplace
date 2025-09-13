using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Application.Interfaces.Repositories;

namespace ProductModule.Application.Characteristic.Queries.GetProductCharacterisitcs
{
    public class GetProductCharacteristicsQueryHandler : IRequestHandler<GetProductCharacteristicsQuery, IEnumerable<CharacteristicGroupDto>>
    {
        private readonly ICharacteristicRepository _characteristicRepository;
        public GetProductCharacteristicsQueryHandler(ICharacteristicRepository characteristicRepository)
        {
            _characteristicRepository = characteristicRepository;
        }

        public async Task<IEnumerable<CharacteristicGroupDto>> Handle(GetProductCharacteristicsQuery query, CancellationToken cancellationToken)
        {
            var resposnse = await _characteristicRepository.GetProductCharacterisitcsAsync(query.ProductId, cancellationToken);

            return resposnse;
        }
    }
}
