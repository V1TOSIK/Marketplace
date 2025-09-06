using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Events;

namespace ProductModule.Application.Product.Commands.ShowUnBanedUserProducts
{
    public class ShowUnBanedUserProductsEventHandler : INotificationHandler<UnbanUserDomainEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductUnitOfWork _productUnitOfWork;
        private readonly ILogger<ShowUnBanedUserProductsEventHandler> _logger;

        public ShowUnBanedUserProductsEventHandler(IProductRepository productRepository,
            IProductUnitOfWork productUnitOfWork,
            ILogger<ShowUnBanedUserProductsEventHandler> logger)
        {
            _logger = logger;
            _productRepository = productRepository;
            _productUnitOfWork = productUnitOfWork;
        }
        
        public async Task Handle(UnbanUserDomainEvent notification, CancellationToken cancellationToken)
        {
            await _productRepository.ShowUserProductsAsync(notification.UserId, cancellationToken);
            await _productUnitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Product Module] All products for unbanned user ID {UserId} have been restored.", notification.UserId);
        }
    }
}
