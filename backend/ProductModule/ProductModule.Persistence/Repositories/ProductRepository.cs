using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces.Repositories;
using ProductModule.Domain.Entities;
using ProductModule.Domain.Enums;
using ProductModule.Domain.Exceptions;


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

        public IQueryable<Product> Query(CancellationToken cancellationToken)
        {
            return _dbContext.Products.AsNoTracking();
        }

        public async Task<IEnumerable<Guid>> GetProductIdsFilteredByCharacteristics(List<(int templateId, IEnumerable<string> values)> filters, CancellationToken cancellationToken)
        {
            var productIds = await _dbContext.CharacteristicValues
             .Where(cv => filters.Any(f =>
                 f.templateId == cv.CharacteristicTemplateId &&
                 f.values.Contains(cv.Value)))
             .Select(cv => cv.GroupId)
             .Distinct()
             .Join(_dbContext.CharacteristicGroups,
                 valGroupId => valGroupId,
                 group => group.Id,
                 (valGroupId, group) => group.ProductId)
             .Distinct()
             .ToListAsync(cancellationToken);

            return productIds;
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken)
        {
            return await _dbContext.Products
                .Where(p => p.CategoryId == categoryId && p.Status == Status.Published)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByUserIdAsync(Guid userId, IEnumerable<Status> statuses, CancellationToken cancellationToken)
        {
            return await _dbContext.Products
                .Where(p => p.UserId == userId && statuses.Contains(p.Status))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Product> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

            if (product == null)
                throw new ProductNotFoundException($"Product with ID {productId} not found.");

            return product;

        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            if (product == null)
            {
                _logger.LogError("Attempted to add a null product.");
                throw new NullableProductException("Product cannot be null.");
            }
            await _dbContext.Products.AddAsync(product, cancellationToken);
            _logger.LogInformation($"Product with ID {product.Id} added successfully.");
        }

        public async Task DeleteAsync(Guid productId, Guid userId, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {productId} not found for deletion.");
                return;
            }

            if (product.UserId != userId)
            {
                _logger.LogWarning($"User {userId} attempted to delete product {productId} they do not own.");
                throw new UnauthorizedAccessException($"User {userId} is not authorized to delete product {productId}.");
            }

            _dbContext.Products.Remove(product);
            _logger.LogInformation($"Product with ID {productId} deleted successfully.");
        }
    }
}
