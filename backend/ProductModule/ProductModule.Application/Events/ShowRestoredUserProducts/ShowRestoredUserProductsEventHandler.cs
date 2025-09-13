using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Events;

namespace ProductModule.Application.Events.ShowRestoredUserProducts
{
    public class ShowRestoredUserProductsEventHandler : INotificationHandler<RestoreUserEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductUnitOfWork _productUnitOfWork;
        private readonly ILogger<ShowRestoredUserProductsEventHandler> _logger;

        public ShowRestoredUserProductsEventHandler(IProductRepository productRepository,
            IProductUnitOfWork productUnitOfWork,
            ILogger<ShowRestoredUserProductsEventHandler> logger)
        {
            _logger = logger;
            _productRepository = productRepository;
            _productUnitOfWork = productUnitOfWork;
        }

        public async Task Handle(RestoreUserEvent notification, CancellationToken cancellationToken)
        {
            await _productRepository.ShowUserProductsAsync(notification.UserId, cancellationToken);
            await _productUnitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Product Module] All products for user ID {UserId} have been restored.", notification.UserId);
        }
    }
}
