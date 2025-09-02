using ProductModule.Domain.Enums;

namespace ProductModule.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        IQueryable<ProductModule.Domain.Entities.Product> Query(CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetProductIdsFilteredByCharacteristics(List<(int templateId, IEnumerable<string> values)> filters, CancellationToken cancellationToken);
        Task<IEnumerable<ProductModule.Domain.Entities.Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
        Task<IEnumerable<ProductModule.Domain.Entities.Product>> GetByUserIdAsync(Guid userId, IEnumerable<Status> statuses, CancellationToken cancellationToken);
        Task<ProductModule.Domain.Entities.Product> GetByIdAsync(Guid productId, CancellationToken cancellationToken);
        Task AddAsync(ProductModule.Domain.Entities.Product product, CancellationToken cancellationToken);
        Task DeleteAsync (Guid productId, Guid userId, CancellationToken cancellationToken);
    }
}
