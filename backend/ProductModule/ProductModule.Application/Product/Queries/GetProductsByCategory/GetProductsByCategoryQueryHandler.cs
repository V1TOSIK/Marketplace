using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Dtos;
using SharedKernel.Pagination;
using SharedKernel.Queries;

namespace ProductModule.Application.Product.Queries.GetProductByCategory
{
    public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, PaginationResponse<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediator _mediator;
        public GetProductsByCategoryQueryHandler(IProductRepository productRepository,
            IMediator mediator)
        {
            _productRepository = productRepository;
            _mediator = mediator;
        }

        public async Task<PaginationResponse<ProductDto>> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
        {
            var products = _productRepository.GetByCategoryIdAsync(query.CategoryId, cancellationToken);

            var paginatedProducts = await products.ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

            var mainMedias = await _mediator.Send(new GetMainMediasQuery(paginatedProducts.Items.Select(p => p.Id).ToList()), cancellationToken);
            var items = paginatedProducts.Items.Select(p =>
            {
                var media = mainMedias.TryGetValue(p.Id, out var result) ? result : new MediaDto();

                return new ProductDto(p.Id,
                    p.UserId,
                    [media],
                    p.Name,
                    p.Price.Amount,
                    p.Price.Currency,
                    p.Location,
                    p.Description,
                    p.CategoryId,
                    p.Status.ToString());
            }).ToList();
            return new PaginationResponse<ProductDto>(items, paginatedProducts.TotalCount, paginatedProducts.PageNumber, paginatedProducts.PageSize);
        }
    }
}
