using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductModule.Application.Dtos;
using ProductModule.Domain.Enums;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Interfaces;

namespace ProductModule.Application.Product.Queries.GetFilteredProducts
{
    public class GetFilteredProductQueryHandler : IRequestHandler<GetFilteredProductQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediaManager _mediaManager;
        public GetFilteredProductQueryHandler(IProductRepository productRepository,
            IMediaManager mediaManager)
        {
            _productRepository = productRepository;
            _mediaManager = mediaManager;
        }
        public async Task<IEnumerable<ProductDto>> Handle(GetFilteredProductQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _productRepository.Query(cancellationToken);

            dbQuery = dbQuery.Where(p => p.Status == Status.Published);
            if (query.MaxPrice.HasValue)
                dbQuery = dbQuery.Where(p => p.Price.Amount <= query.MaxPrice);
            if (query.MinPrice.HasValue)
                dbQuery = dbQuery.Where(p => p.Price.Amount >= query.MinPrice);

            if (query.Location?.Any() == true)
                dbQuery = dbQuery.Where(p => query.Location.Contains(p.Location));

            if (query.CategoryId.HasValue)
                dbQuery = dbQuery.Where(p => p.CategoryId == query.CategoryId);

            if (query.Characteristics?.Any() == true)
            {
                var queries = query.Characteristics
                    .Select(c => (c.TemplateId, c.Values))
                    .ToList();
                var productIds = await _productRepository
                    .GetProductIdsFilteredByCharacteristics(queries, cancellationToken);
                dbQuery = dbQuery.Where(p => productIds.Contains(p.Id));
            }

            var result = await dbQuery.ToListAsync(cancellationToken);

            var mainMediaUrls = await _mediaManager
                .GetAllMainMediaUrls(result.Select(r => r.Id), cancellationToken);

            var response = result.Select(p =>
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
