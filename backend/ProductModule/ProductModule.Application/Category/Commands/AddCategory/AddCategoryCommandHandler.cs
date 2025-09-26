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
            var category = Domain.Entities.Category.Create(command.Name);
            _logger.LogInformation("[Product Module] Adding new category with name: {categoryName}", command.Name);

            await _categoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Product Module] Category with ID {categoryId} added successfully.", category.Id);
        }
    }
}
