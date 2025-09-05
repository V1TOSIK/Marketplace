using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Application.Interfaces.Repositories;
using ProductModule.Domain.Enums;
using SharedKernel.Interfaces;

namespace ProductModule.Application.Product.Queries.GetUserProducts
{
    public class GetUserProductsQueryHandler : IRequestHandler<GetUserProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediaManager _mediaManager;
        public GetUserProductsQueryHandler(IProductRepository productRepository,
            IMediaManager mediaManager)
        {
            _productRepository = productRepository;
            _mediaManager = mediaManager;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetUserProductsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<Status> statuses = [Status.Published];
            var products = await _productRepository.GetByUserIdAsync(query.UserId, statuses, cancellationToken);

            var mainMediaUrls = await _mediaManager
                .GetAllMainMediaUrls(products.Select(p => p.Id), cancellationToken);

            var response = products.Select(p =>
            {
                var url = mainMediaUrls.TryGetValue(p.Id, out var result) ? result : string.Empty;

                return new ProductDto
                {
                    Id = p.Id,
                    MainMediaUrl = url,
                    Name = p.Name,
                    PriceCurrency = p.Price.Currency,
                    PriceAmount = p.Price.Amount,
                    Location = p.Location,
                    CategoryId = p.CategoryId,
                    UserId = p.UserId
                };
            });
            return response;
        }
    }
}
