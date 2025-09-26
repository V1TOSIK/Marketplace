using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Interfaces.Repositories;

namespace ProductModule.Application.Category.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;
        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository,
            IProductUnitOfWork unitOfWork,
            ILogger<UpdateCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
            if (string.IsNullOrWhiteSpace(command.Request.Name))
            {
                _logger.LogError("[Product Module] Attempted to update category with ID {categoryId} to a null or empty name.", command.CategoryId);
                throw new ArgumentNullException(nameof(command.Request.Name), "Category name cannot be null or empty.");
            }
            category.UpdateName(command.Request.Name);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Product Module] Category with ID {categoryId} updated successfully.", command.CategoryId);
        }
    }
}
