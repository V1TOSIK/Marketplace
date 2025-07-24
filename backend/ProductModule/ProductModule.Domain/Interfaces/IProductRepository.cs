using ProductModule.Domain.Entities;
using ProductModule.Domain.Enums;

namespace ProductModule.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        IQueryable<Product> Query();
        Task<IEnumerable<Guid>> GetProductIdsFilteredByCharacteristics(List<(int templateId, List<string> values)> filters);
        Task<IEnumerable<Product>> GetByUserIdWithDraftsAsync(Guid userId);
        Task<IEnumerable<Product>> GetByStatus(Status status);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Product>> GetByUserIdAsync(Guid userId);
        Task<Product> GetByIdAsync(Guid productId);
        Task AddAsync(Product product);
        Task DeleteAsync (Guid productId, Guid userId);
    }
}
