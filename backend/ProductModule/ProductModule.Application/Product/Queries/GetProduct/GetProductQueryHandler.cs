using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Queries;

namespace ProductModule.Application.Product.Queries.GetProduct
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediator _mediator;

        public GetProductQueryHandler(IProductRepository productRepository,
            IMediator mediator)
        {
            _productRepository = productRepository;
            _mediator = mediator;
        }
        public async Task<ProductDto> Handle(GetProductQuery query, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(query.ProductId, cancellationToken);
            var entityMediaUrls = await _mediator.Send(new GetEntityMediasQuery(product.Id), cancellationToken);

            var mainMediaUrl = entityMediaUrls.FirstOrDefault() ?? string.Empty;

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
                UserId = product.UserId,
                Status = product.Status.ToString()
            };

            return response;
        }
    }
}
