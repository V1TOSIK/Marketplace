using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Application.Interfaces.Repositories;
using ProductModule.Domain.Enums;
using SharedKernel.Dtos;
using SharedKernel.Queries;

namespace ProductModule.Application.Product.Queries.GetUserProducts
{
    public class GetUserProductsQueryHandler : IRequestHandler<GetUserProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediator _mediator;
        public GetUserProductsQueryHandler(IProductRepository productRepository,
            IMediator mediator)
        {
            _productRepository = productRepository;
            _mediator = mediator;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetUserProductsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<Status> statuses = [Status.Published];
            var products = await _productRepository.GetByUserIdAsync(query.UserId, statuses, cancellationToken);

            var mainMedias = await _mediator.Send(new GetMainMediasQuery(products.Select(p => p.Id)), cancellationToken);

            var response = products.Select(p =>
            {
                var media = mainMedias.TryGetValue(p.Id, out var result) ? result : new MediaDto();

                return new ProductDto
                {
                    Id = p.Id,
                    Medias = [media],
                    Name = p.Name,
                    PriceCurrency = p.Price.Currency,
                    PriceAmount = p.Price.Amount,
                    Location = p.Location,
                    CategoryId = p.CategoryId,
                    UserId = p.UserId,
                    Status = p.Status.ToString()
                };
            });
            return response;
        }
    }
}
