using ProductModule.Application.Dtos.Requests;
using ProductModule.Application.Dtos.Responses;

namespace ProductModule.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetProductsByFilterAsync(ProductFilterRequest filter);
        Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
        Task<IEnumerable<ProductResponse>> GetProductsByCategoryIdAsync(int categoryId);
        Task<IEnumerable<ProductResponse>> GetProductsByUserIdAsync(Guid userId);
        Task<IEnumerable<ProductResponse>> GetMyProducts(Guid userId);
        Task<CurrentProductResponse> GetProductByIdAsync(Guid productId);
        Task PublishProductAsync(Guid productId, Guid userId);
        Task AddProductAsync(Guid userId, AddProductRequest request);
        Task DeleteProductAsync(Guid productId, Guid userId);
    }
}
