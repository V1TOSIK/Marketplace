using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Dtos;
using ProductModule.Application.Interfaces.Repositories;
using ProductModule.Domain.Entities;
using ProductModule.Domain.Exceptions;

namespace ProductModule.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {

        private readonly ProductDbContext _dbContext;
        private readonly ILogger<CategoryRepository> _logger;
        public CategoryRepository(ProductDbContext dbContext,
            ILogger<CategoryRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Category> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
            if (category == null)
            {
                _logger.LogWarning("[Product Module(Repository)] Category with ID {categoryId} not found.", categoryId);
                throw new CategoryNotFoundException($"Category with ID {categoryId} not found.");
            }
            return category;
        }

        public async Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            if (category == null)
            {
                _logger.LogError("[Product Module(Repository)] Attempted to add a null category.");
                throw new NullableCategoryException("Category cannot be null.");
            }
            await _dbContext.Categories.AddAsync(category, cancellationToken);
        }

        public async Task DeleteAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
            if (category == null)
            {
                _logger.LogWarning("[Product Module(Repository)] Category with ID {categoryId} not found.", categoryId);
                throw new CategoryNotFoundException($"Category with ID {categoryId} not found.");
            }
            _dbContext.Categories.Remove(category);
        }
    }
}
