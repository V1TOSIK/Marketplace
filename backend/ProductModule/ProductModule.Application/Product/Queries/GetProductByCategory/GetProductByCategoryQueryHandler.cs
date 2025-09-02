using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Interfaces;

namespace ProductModule.Application.Product.Queries.GetProductByCategory
{
    public class GetProductByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediaManager _mediaManager;
        public GetProductByCategoryQueryHandler(IProductRepository productRepository,
            IMediaManager mediaManager)
        {
            _productRepository = productRepository;
            _mediaManager = mediaManager;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetByCategoryIdAsync(query.CategoryId, cancellationToken);
            var mainMediaUrls = await _mediaManager
                .GetAllMainMediaUrls(products.Select(p => p.Id), cancellationToken);
            var response = products.Select(p => new ProductDto
            {
                Id = p.Id,
                MainMediaUrl = mainMediaUrls.TryGetValue(p.Id, out var result) ? result : string.Empty,
                Name = p.Name,
                PriceCurrency = p.Price.Currency,
                PriceAmount = p.Price.Amount,
                Location = p.Location,
                CategoryId = p.CategoryId,
                UserId = p.UserId
            });
            return response;
        }
    }
}
