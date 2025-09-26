using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Application.Interfaces.Repositories;
using ProductModule.Domain.Enums;
using SharedKernel.Dtos;
using SharedKernel.Pagination;
using SharedKernel.Queries;

namespace ProductModule.Application.Product.Queries.GetUserProducts
{
    public class GetUserProductsQueryHandler : IRequestHandler<GetUserProductsQuery, PaginationResponse<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediator _mediator;
        public GetUserProductsQueryHandler(IProductRepository productRepository,
            IMediator mediator)
        {
            _productRepository = productRepository;
            _mediator = mediator;
        }

        public async Task<PaginationResponse<ProductDto>> Handle(GetUserProductsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<Status> statuses = [Status.Published];
            var products = _productRepository.GetByUserIdAsync(query.UserId, statuses, cancellationToken);

            var paginatedProducts = await products.ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

            var mainMedias = await _mediator.Send(new GetMainMediasQuery(products.Select(p => p.Id).ToList()), cancellationToken);

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
