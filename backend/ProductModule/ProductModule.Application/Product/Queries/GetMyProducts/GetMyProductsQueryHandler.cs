using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Dtos;
using ProductModule.Application.Interfaces.Repositories;
using ProductModule.Domain.Enums;
using ProductModule.SharedKernel.Interfaces;
using SharedKernel.Interfaces;
using SharedKernel.Queries;

namespace ProductModule.Application.Product.Queries.GetMyProducts
{
    public class GetMyProductsQueryHandler : IRequestHandler<GetMyProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetMyProductsQueryHandler> _logger;

        public GetMyProductsQueryHandler(IProductRepository productRepository,
            IMediator mediator,
            ICurrentUserService currentUserService,
            ILogger<GetMyProductsQueryHandler> logger)
        {
            _productRepository = productRepository;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetMyProductsQuery query, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                _logger.LogWarning("[Product Module] User ID is empty. Cannot retrieve products.");
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var parsedStatuses = query.GetParsedStatuses();
            var statuses = parsedStatuses?.ToList() ?? new List<Status>
            {
                Status.Published,
                Status.Draft
            };
            var products = await _productRepository.GetByUserIdAsync(userId.Value, statuses, cancellationToken);
            var mainMediaUrls = await _mediator.Send(new GetMainMediasQuery(products.Select(p => p.Id)), cancellationToken);
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
