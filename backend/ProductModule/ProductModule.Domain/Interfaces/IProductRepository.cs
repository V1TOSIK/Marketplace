using ProductModule.Domain.Entities;

namespace ProductModule.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Product>> GetByUserIdAsync(Guid userId);
        Task<Product> GetByIdAsync(Guid productId);
        Task AddAsync(Product product);
        Task DeleteAsync (Guid productId);
    }
}
