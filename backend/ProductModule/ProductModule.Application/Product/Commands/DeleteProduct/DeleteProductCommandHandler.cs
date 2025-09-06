using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Interfaces.Repositories;
using ProductModule.SharedKernel.Interfaces;

namespace ProductModule.Application.Product.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductUnitOfWork _productUnitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(
            IProductRepository productRepository,
            IProductUnitOfWork productUnitOfWork,
            ICurrentUserService currentUserService,
            ILogger<DeleteProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _productUnitOfWork = productUnitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                _logger.LogWarning("[Product Module] User ID is empty. Cannot delete product.");
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            await _productRepository.DeleteAsync(request.ProductId, userId.Value, cancellationToken);
            await _productUnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
