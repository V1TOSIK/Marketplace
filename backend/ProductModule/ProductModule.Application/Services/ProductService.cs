using Microsoft.Extensions.Logging;
using ProductModule.Application.Dtos.Requests;
using ProductModule.Application.Dtos.Responses;
using ProductModule.Application.Interfaces;
using ProductModule.Domain.Entities;
using ProductModule.Domain.Interfaces;
using SharedKernel.Interfaces;

namespace ProductModule.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICharacteristicRepository _characteristicRepository;
        private readonly IProductUnitOfWork _productUnitOfWork;
        private readonly ILogger<ProductService> _logger;
        public ProductService(IProductRepository productRepository,
            ICharacteristicRepository characteristicRepository,
            IProductUnitOfWork productUnitOfWork,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _characteristicRepository = characteristicRepository;
            _productUnitOfWork = productUnitOfWork;
            _logger = logger;
        }

        public async Task AddProductAsync(Guid userId, AddProductRequest request)
        {
            var product = Product.Create(
                userId,
                request.Name,
                request.PriceCurrency,
                request.PriceAmount,
                request.Location,
                request.Description,
                request.CategoryId);

            if (request.CharacteristicGroups != null && request.CharacteristicGroups.Any())
            {
                foreach (var group in request.CharacteristicGroups)
                {
                    var characteristicGroup = CharacteristicGroup.Create(group.Name, product.Id);
                    await AddCharacteristicValues(
                        characteristicGroup,
                        group.CharacteristicValues,
                        request.CategoryId);

                    product.AddCharacteristicGroup(characteristicGroup);
                }
            }

            await _productRepository.AddAsync(product);
            await _productUnitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product '{ProductName}' (ID: {ProductId}) successfully created", product.Name, product.Id);
        }

        private async Task AddCharacteristicValues(
            CharacteristicGroup characteristicGroup,
            IEnumerable<CharacteristicValueRequest>? characteristicValues,
            int categoryId)
        {
            if (characteristicValues == null || !characteristicValues.Any())
                return;
            foreach (var value in characteristicValues)
            {
                var template = await _characteristicRepository.GetOrCreateTemplateAsync(value.Name, categoryId, value.Unit);
                characteristicGroup.AddCharacteristic(value.Value, template.Id);
            }
        }

        public Task DeleteProductAsync(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductResponse> GetProductByIdAsync(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductResponse>> GetProductsByCategoryIdAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductResponse>> GetProductsByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
