using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Domain.Enums;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Interfaces;
using SharedKernel.Specification;
using SharedKernel.Queries;
using SharedKernel.Dtos;
using SharedKernel.Pagination;

namespace ProductModule.Application.Product.Queries.GetFilteredProducts
{
    public class GetFilteredProductQueryHandler : IRequestHandler<GetFilteredProductQuery, PaginationResponse<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediator _mediator;
        public GetFilteredProductQueryHandler(IProductRepository productRepository, IMediator mediator)
        {
            _productRepository = productRepository;
            _mediator = mediator;
        }
        public async Task<PaginationResponse<ProductDto>> Handle(GetFilteredProductQuery query, CancellationToken cancellationToken)
        {
            var spec = new Specification<Domain.Entities.Product>();
            spec.AddCriteria(p => p.Status == Status.Published);

            if (query.MaxPrice.HasValue)
                spec.AddCriteria(p => p.Price.Amount <= query.MaxPrice);
            if (query.MinPrice.HasValue)
                spec.AddCriteria(p => p.Price.Amount >= query.MinPrice);

            if (query.Location?.Any() == true)
                spec.AddCriteria(p => query.Location.Contains(p.Location));

            if (query.CategoryId.HasValue)
                spec.AddCriteria(p => p.CategoryId == query.CategoryId);

            if (query.Characteristics?.Any() == true)
            {
                var queries = query.Characteristics
                    .Select(c => (c.TemplateId, c.Values))
                    .ToList();
                var productIds = await _productRepository
                    .GetProductIdsFilteredByCharacteristics(queries, cancellationToken);
                spec.AddCriteria(p => productIds.Contains(p.Id));
            }

            if (!string.IsNullOrEmpty(query.SortedBy))
            {
                spec.ApplyOrderByProperty(query.SortedBy, query.SortDescending);
            }

            var queryable = _productRepository.AsQueryable(spec, cancellationToken);

            var paginatedResult = await queryable.ToPaginatedListAsync(
                query.PageNumber,
                query.PageSize,
                cancellationToken
            );

            var mainMedias = await _mediator.Send(new GetMainMediasQuery(paginatedResult.Items.Select(r => r.Id)), cancellationToken);

            var items = paginatedResult.Items.Select(p =>
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
            return new PaginationResponse<ProductDto>(items, paginatedResult.TotalCount, paginatedResult.PageNumber, paginatedResult.PageSize);
        }
    }
}
