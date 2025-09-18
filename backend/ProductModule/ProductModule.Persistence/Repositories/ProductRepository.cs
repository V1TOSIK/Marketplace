using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces.Repositories;
using ProductModule.Domain.Entities;
using ProductModule.Domain.Enums;
using ProductModule.Domain.Exceptions;
using SharedKernel.Specification;

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

        public async Task ShowUserProductsAsync(Guid userId, CancellationToken cancellationToken)
        {
            var products = await _dbContext.Products
                .Where(p => p.UserId == userId && p.Status != Status.Published)
                .ToListAsync(cancellationToken);
            foreach (var product in products)
            {
                product.UpdateStatus(Status.Published);
            }
        }

        public async Task HideUserProductsAsync(Guid userId, CancellationToken cancellationToken)
        {
            var products = await _dbContext.Products
                .Where(p => p.UserId == userId && p.Status == Status.Published)
                .ToListAsync(cancellationToken);
            foreach (var product in products)
            {
                product.UpdateStatus(Status.Hidden);
            }
        }

        public async Task<IEnumerable<Product>> GetBySpecificationAsync(Specification<Product> spec, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = _dbContext.Products.AsNoTracking();

            foreach (var criteria in spec.Criteria)
                query = query.Where(criteria);

            foreach (var include in spec.Includes)
                query = query.Include(include);

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if (spec.IsPagingEnabled && spec.PageNumber.HasValue && spec.PageSize.HasValue)
                query = query.Skip(spec.PageNumber.Value).Take(spec.PageSize.Value);

            return await query.ToListAsync(cancellationToken);
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
            var query = _dbContext.Products
                .Where(p => p.UserId == userId)
                .AsNoTracking();

            if (statuses != null && statuses.Any())
            {
                query = query.Where(p => statuses.Contains(p.Status));
            }

            return await query.ToListAsync(cancellationToken);
        }

        //only for command not for query
        public async Task<Product> GetByIdWithCharacteristicsAsync(Guid productId, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products
                .Include(p => p.CharacteristicGroups)
                    .ThenInclude(g => g.CharacteristicValues)
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("[Product Module(Repository)] Product with ID {productId} not found.", productId);
                throw new ProductNotFoundException($"Product with ID {productId} not found.");
            }
            return product;
        }

        public async Task<Product> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

            if (product == null)
            {
                _logger.LogWarning("[Product Module(Repository)] Product with ID {productId} not found.", productId);
                throw new ProductNotFoundException($"Product with ID {productId} not found.");
            }

            return product;

        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            if (product == null)
            {
                _logger.LogError("[Product Module(Repository)] Attempted to add a null product.");
                throw new NullableProductException("Product cannot be null.");
            }
            await _dbContext.Products.AddAsync(product, cancellationToken);
        }

        public async Task DeleteAsync(Guid productId, Guid userId, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("[Product Module(Repository)] Product with ID {productId} not found for deletion.", productId);
                return;
            }

            if (product.UserId != userId)
            {
                _logger.LogWarning("[Product Module(Repository)] User {userId} attempted to delete product {productId} they do not own.", userId, productId);
                throw new UnauthorizedAccessException($"User {userId} is not authorized to delete product {productId}.");
            }

            _dbContext.Products.Remove(product);
        }

        public async Task DeleteUserProductsAsync(Guid userId, CancellationToken cancellationToken)
        {
            var products = await _dbContext.Products
                .Where(p => p.UserId == userId)
                .ToListAsync(cancellationToken);
            if (products.Count == 0)
            {
                _logger.LogInformation("[Product Module(Repository)] No products found for user {userId} to delete.", userId);
                return;
            }
            _dbContext.Products.RemoveRange(products);
        }

        public async Task DeleteByCategoryAsync(int categoryId, CancellationToken cancellationToken)
        {
            var products = await _dbContext.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync(cancellationToken);
            if (products.Count == 0)
            {
                _logger.LogInformation("[Product Module(Repository)] No products found for category {categoryId} to delete.", categoryId);
                return;
            }
            _dbContext.Products.RemoveRange(products);
        }
    }
}
