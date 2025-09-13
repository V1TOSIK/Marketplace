using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Events;

namespace ProductModule.Application.Events.HideBannedUserProducts
{
    public class HideBannedUserProductsEventHandler : INotificationHandler<BanUserEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductUnitOfWork _productUnitOfWork;
        private readonly ILogger<HideBannedUserProductsEventHandler> _logger;

        public HideBannedUserProductsEventHandler(IProductRepository productRepository,
            IProductUnitOfWork productUnitOfWork,
            ILogger<HideBannedUserProductsEventHandler> logger)
        {
            _productRepository = productRepository;
            _productUnitOfWork = productUnitOfWork;
            _logger = logger;
        }

        public async Task Handle(BanUserEvent notification, CancellationToken cancellationToken)
        {
            await _productRepository.HideUserProductsAsync(notification.UserId, cancellationToken);
            await _productUnitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Product Module] All products for banned user ID {UserId} have been hidden.", notification.UserId);
        }
    }
}
