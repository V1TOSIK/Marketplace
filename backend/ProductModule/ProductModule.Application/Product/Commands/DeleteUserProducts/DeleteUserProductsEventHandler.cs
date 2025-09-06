using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Events;

namespace ProductModule.Application.Product.Commands.DeleteUserProducts
{
    public class DeleteUserProductsEventHandler : INotificationHandler<HardDeleteUserDomainEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductUnitOfWork _productUnitOfWork;
        private readonly ILogger<DeleteUserProductsEventHandler> _logger;

        public DeleteUserProductsEventHandler(IProductRepository productRepository,
            IProductUnitOfWork productUnitOfWork,
            ILogger<DeleteUserProductsEventHandler> logger)
        {
            _logger = logger;
            _productRepository = productRepository;
            _productUnitOfWork = productUnitOfWork;
        }

        public async Task Handle(HardDeleteUserDomainEvent notification, CancellationToken cancellationToken)
        {
            await _productRepository.DeleteUserProductsAsync(notification.UserId, cancellationToken);
            await _productUnitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Product Module] All products for user ID {UserId} have been deleted.", notification.UserId);
        }
    }
}
