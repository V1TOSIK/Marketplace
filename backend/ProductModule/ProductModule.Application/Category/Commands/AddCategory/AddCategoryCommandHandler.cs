using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Interfaces.Repositories;

namespace ProductModule.Application.Category.Commands.AddCategory
{
    public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductUnitOfWork _unitOfWork;
        private readonly ILogger<AddCategoryCommandHandler> _logger;

        public AddCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            IProductUnitOfWork unitOfWork,
            ILogger<AddCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(AddCategoryCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                _logger.LogError("[Product Module] Attempted to add a category with null or empty name.");
                throw new ArgumentNullException(nameof(command.Name), "Category name cannot be null or empty.");
            }
            var category = Domain.Entities.Category.Create(command.Name);

            await _categoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Product Module] Category with ID {categoryId} added successfully.", category.Id);
        }
    }
}
