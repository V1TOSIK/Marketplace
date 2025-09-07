using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Events;

namespace ProductModule.Application.Events.HideDeletedUserProducts
{
    public class HideDeletedUserProductsEventHandler : INotificationHandler<SoftDeleteUserDomainEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductUnitOfWork _productUnitOfWork;
        private readonly ILogger<HideDeletedUserProductsEventHandler> _logger;
        public HideDeletedUserProductsEventHandler(IProductRepository productRepository,
            IProductUnitOfWork productUnitOfWork,
            ILogger<HideDeletedUserProductsEventHandler> logger)
        {
            _logger = logger;
            _productRepository = productRepository;
            _productUnitOfWork = productUnitOfWork;
        }
        public async Task Handle(SoftDeleteUserDomainEvent notification, CancellationToken cancellationToken)
        {
            await _productRepository.HideUserProductsAsync(notification.UserId, cancellationToken);
            await _productUnitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Product Module] All products for user ID {UserId} have been hidden.", notification.UserId);
        }
    }
}
