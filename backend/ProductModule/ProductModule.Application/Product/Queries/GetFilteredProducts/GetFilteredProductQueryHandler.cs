using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Domain.Enums;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Interfaces;
using SharedKernel.Specification;
using SharedKernel.Queries;
using SharedKernel.Dtos;

namespace ProductModule.Application.Product.Queries.GetFilteredProducts
{
    public class GetFilteredProductQueryHandler : IRequestHandler<GetFilteredProductQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediator _mediator;
        public GetFilteredProductQueryHandler(IProductRepository productRepository, IMediator mediator)
        {
            _productRepository = productRepository;
            _mediator = mediator;
        }
        public async Task<IEnumerable<ProductDto>> Handle(GetFilteredProductQuery query, CancellationToken cancellationToken)
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

            if (query.PageSize.HasValue && query.Page.HasValue)
                spec.ApplyPaging((query.Page.Value - 1) * query.PageSize.Value, query.PageSize.Value);

            var result = await _productRepository.GetBySpecificationAsync(spec, cancellationToken);

            var mainMedias = await _mediator.Send(new GetMainMediasQuery(result.Select(r => r.Id)), cancellationToken);

            var response = result.Select(p =>
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
