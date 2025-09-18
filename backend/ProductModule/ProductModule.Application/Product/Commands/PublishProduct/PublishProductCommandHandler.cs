using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Interfaces;

namespace ProductModule.Application.Product.Commands.PublishProduct
{
    public class PublishProductCommandHandler : IRequestHandler<PublishProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductUnitOfWork _productUnitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<PublishProductCommandHandler> _logger;

        public PublishProductCommandHandler(IProductRepository productRepository,
            IProductUnitOfWork productUnitOfWork,
            ICurrentUserService currentUserService,
            ILogger<PublishProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _productUnitOfWork = productUnitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Handle(PublishProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("[Product Module] Product with ID {ProductId} not found", command.ProductId);
                throw new KeyNotFoundException($"Product with ID {command.ProductId} not found.");
            }
            var userId = _currentUserService.UserId;
            if (product.UserId != userId)
            {
                _logger.LogWarning("[Product Module] User {UserId} is not authorized to publish product {ProductId}", userId, command.ProductId);
                throw new UnauthorizedAccessException("You are not authorized to publish this product.");
            }
            product.Publish();
            await _productUnitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Product Module] Product '{ProductName}' (ID: {ProductId}) successfully published", product.Name, product.Id);
        }
    }
}
