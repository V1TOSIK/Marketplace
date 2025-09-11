using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Interfaces.Repositories;

namespace ProductModule.Application.Category.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;
        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository,
            IProductUnitOfWork unitOfWork,
            ILogger<DeleteCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
                _logger.LogInformation("[Product Module] Deleting category with ID {categoryId}.", command.CategoryId);
                category.Delete();
                _logger.LogInformation("Domain events count: {count}", category.DomainEvents.Count);
                _logger.LogInformation("[Product Module] Category with ID {categoryId} marked as deleted.", command.CategoryId);
                await _categoryRepository.DeleteAsync(command.CategoryId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            _logger.LogInformation("[Product Module] Category with ID {categoryId} deleted successfully.", command.CategoryId);
        }
    }
}
