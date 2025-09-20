using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Queries;
using SharedKernel.Dtos;

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
            var entityMedias = await _mediator.Send(new GetEntityMediasQuery(product.Id), cancellationToken);

            var response =  new ProductDto(product.Id,
                    product.UserId,
                    entityMedias,
                    product.Name,
                    product.Price.Amount,
                    product.Price.Currency,
                    product.Location,
                    product.Description,
                    product.CategoryId,
                    product.Status.ToString());

            return response;
        }
    }
}
