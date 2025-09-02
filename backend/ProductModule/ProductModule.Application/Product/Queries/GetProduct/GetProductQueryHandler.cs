using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Domain.Entities;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Interfaces;

namespace ProductModule.Application.Product.Queries.GetProduct
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediaManager _mediaManager;

        public GetProductQueryHandler(IProductRepository productRepository,
            IMediaManager mediaManager)
        {
            _productRepository = productRepository;
            _mediaManager = mediaManager;
        }
        public async Task<ProductDto> Handle(GetProductQuery query, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(query.ProductId, cancellationToken);
            var mainMediaUrl = await _mediaManager.GetMainMediaUrl(product.Id, cancellationToken);
            var entityMediaUrls = await _mediaManager.GetAllEntityMediaUrls(product.Id, cancellationToken);

            var response = new ProductDto
            {
                Id = product.Id,
                MainMediaUrl = mainMediaUrl,
                MediaUrls = entityMediaUrls,
                Name = product.Name,
                PriceCurrency = product.Price.Currency,
                PriceAmount = product.Price.Amount,
                Location = product.Location,
                Description = product.Description,
                CategoryId = product.CategoryId,
                UserId = product.UserId
            };

            return response;
        }
    }
}
