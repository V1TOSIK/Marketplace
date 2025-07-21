using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductModule.Domain.Entities;
using ProductModule.Domain.Exceptions;
using ProductModule.Domain.Interfaces;

namespace ProductModule.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _dbContext;
        private readonly ILogger<ProductRepository> _logger;
        public ProductRepository(ProductDbContext dbContext,
            ILogger<ProductRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return await _dbContext.Products
                .Where(p => p.CategoryId == categoryId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.Products
                .Where(p => p.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(Guid productId)
        {
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new ProductNotFoundException($"Product with ID {productId} not found.");

            return product;

        }

        public async Task AddAsync(Product product)
        {
            if (product == null)
            {
                _logger.LogError("Attempted to add a null product.");
                throw new NullableProductException("Product cannot be null.");
            }
            await _dbContext.Products.AddAsync(product);
            _logger.LogInformation($"Product with ID {product.Id} added successfully.");
        }

        public async Task DeleteAsync(Guid productId)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {productId} not found for deletion.");
                return;
            }
            _dbContext.Products.Remove(product);
            _logger.LogInformation($"Product with ID {productId} deleted successfully.");
        }
    }
}
