using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Interfaces.Repositories;
using SharedKernel.Events;

namespace ProductModule.Application.Events.DeleteCategoryProducts
{
    public class DeleteCategoryProductsEventHandler : INotificationHandler<DeleteCategoryDomainEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCategoryProductsEventHandler> _logger;

        public DeleteCategoryProductsEventHandler(IProductRepository productRepository,
            IProductUnitOfWork unitOfWork,
            ILogger<DeleteCategoryProductsEventHandler> logger)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeleteCategoryDomainEvent notification, CancellationToken cancellationToken)
        {

            await _productRepository.DeleteByCategoryAsync(notification.CategoryId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Product Module (Event)] All products of category with Id:{CategoryId} have been deleted.", notification.CategoryId);
        }
    }
}
