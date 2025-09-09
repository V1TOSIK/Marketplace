using ProductModule.Application.Dtos;

namespace ProductModule.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<Domain.Entities.Category> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Entities.Category category, CancellationToken cancellationToken = default);
        Task DeleteAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}
