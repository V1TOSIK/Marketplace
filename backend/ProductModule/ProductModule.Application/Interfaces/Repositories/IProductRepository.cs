using ProductModule.Domain.Enums;
using SharedKernel.Specification;

namespace ProductModule.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Domain.Entities.Product> GetByIdAsync(Guid productId, CancellationToken cancellationToken);
        Task<Domain.Entities.Product> GetByIdWithCharacteristicsAsync(Guid productId, CancellationToken cancellationToken);
        Task<IEnumerable<Domain.Entities.Product>> GetBySpecificationAsync(Specification<Domain.Entities.Product> spec, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetProductIdsFilteredByCharacteristics(List<(int templateId, IEnumerable<string> values)> filters, CancellationToken cancellationToken);
        Task<IEnumerable<Domain.Entities.Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
        Task<IEnumerable<Domain.Entities.Product>> GetByUserIdAsync(Guid userId, IEnumerable<Status> statuses, CancellationToken cancellationToken);
        Task AddAsync(Domain.Entities.Product product, CancellationToken cancellationToken);
        Task ShowUserProductsAsync(Guid userId, CancellationToken cancellationToken);
        Task HideUserProductsAsync(Guid userId, CancellationToken cancellationToken);
        Task DeleteAsync (Guid productId, Guid userId, CancellationToken cancellationToken);
        Task DeleteUserProductsAsync(Guid userId, CancellationToken cancellationToken);
        Task DeleteByCategoryAsync(int categoryId, CancellationToken cancellationToken);
    }
}
