using ProductModule.Application.Dtos.Requests;
using ProductModule.Application.Dtos.Responses;

namespace ProductModule.Application.Interfaces
{
    public interface IProductService
    {
        Task AddProductAsync(Guid userId, AddProductRequest request);
        Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
        Task<IEnumerable<ProductResponse>> GetProductsByCategoryIdAsync(int categoryId);
        Task<IEnumerable<ProductResponse>> GetProductsByUserIdAsync(string userId);
        Task<ProductResponse> GetProductByIdAsync(Guid productId);
        Task DeleteProductAsync(Guid productId);
    }
}
