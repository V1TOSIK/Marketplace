using ProductModule.Domain.Entities;
using ProductModule.Domain.Enums;
using SharedKernel.Pagination;
using SharedKernel.Specification;

namespace ProductModule.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Domain.Entities.Product> GetByIdAsync(Guid productId, CancellationToken cancellationToken);
        Task<Domain.Entities.Product> GetByIdWithCharacteristicsAsync(Guid productId, CancellationToken cancellationToken);
        IQueryable<Domain.Entities.Product> AsQueryable(Specification<Domain.Entities.Product> spec, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetProductIdsFilteredByCharacteristics(List<(int templateId, List<string> values)> filters, CancellationToken cancellationToken);
        IQueryable<Domain.Entities.Product> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
        IQueryable<Domain.Entities.Product> GetByUserIdAsync(Guid userId, IEnumerable<Status> statuses, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetProductIdsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(Domain.Entities.Product product, CancellationToken cancellationToken);
        Task ShowUserProductsAsync(Guid userId, CancellationToken cancellationToken);
        Task HideUserProductsAsync(Guid userId, CancellationToken cancellationToken);
        Task DeleteAsync (Guid productId, Guid userId, CancellationToken cancellationToken);
        Task DeleteProductsByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken);
        Task DeleteUserProductsAsync(Guid userId, CancellationToken cancellationToken);
        Task DeleteByCategoryAsync(int categoryId, CancellationToken cancellationToken);
    }
}
