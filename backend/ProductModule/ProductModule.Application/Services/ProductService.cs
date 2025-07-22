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

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();

            var response = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                PriceCurrency = p.Price.Currency,
                PriceAmount = p.Price.Amount,
                Location = p.Location,
                Description = p.Description,
                CategoryId = p.CategoryId,
                UserId = p.UserId
            });

            return response;
        }

        public async Task<ProductResponse> GetProductByIdAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            var characteristicGroups = await _characteristicRepository
                .GetProductGroupsAsync(productId);

            var groupResponse = new List<CharacteristicGroupResponse>();

            foreach (var characteristicGroup in characteristicGroups)
            {
                var values = await _characteristicRepository
                    .GetValuesByGroupIdAsync(characteristicGroup.Id);

                var templateIds = values.Select(v => v.CharacteristicTemplateId).Distinct().ToList();

                var templates = await _characteristicRepository
                    .GetTemplatesByIdsAsync(templateIds);

                var templatesDictionary = templates.ToDictionary(t => t.Id, t => t);



                var group = new CharacteristicGroupResponse
                {
                    Id = characteristicGroup.Id,
                    Name = characteristicGroup.Name,
                    Characteristics = values
                        .Where(v => v.GroupId == characteristicGroup.Id)
                        .Select(v =>
                        {
                            var template = templatesDictionary[v.CharacteristicTemplateId];
                            return new CharacteristicResponse
                            {
                                Name = template.Name,
                                Value = v.Value,
                                Unit = template.Unit,
                            };
                        }).ToList()
                };
                groupResponse.Add(group);
            }

            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                PriceCurrency = product.Price.Currency,
                PriceAmount = product.Price.Amount,
                Location = product.Location,
                Description = product.Description,
                CategoryId = product.CategoryId,
                UserId = product.UserId,
                CharacteristicGroups = groupResponse
            };


            return response;
        }

        public async Task<IEnumerable<ProductResponse>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var products = await _productRepository.GetByCategoryIdAsync(categoryId);
            var response = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                PriceCurrency = p.Price.Currency,
                PriceAmount = p.Price.Amount,
                Location = p.Location,
                Description = p.Description,
                CategoryId = p.CategoryId,
                UserId = p.UserId
            });
            return response;
        }

        public async Task<IEnumerable<ProductResponse>> GetProductsByUserIdAsync(Guid userId)
        {
            var products = await _productRepository.GetByUserIdAsync(userId);
            var response = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                PriceCurrency = p.Price.Currency,
                PriceAmount = p.Price.Amount,
                Location = p.Location,
                Description = p.Description,
                CategoryId = p.CategoryId,
                UserId = p.UserId
            });
            return response;
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

            if (request.CharacteristicGroups?.Any() == true)
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
            if (!(characteristicValues?.Any() ?? false))
                return;

            foreach (var value in characteristicValues)
            {
                var template = await _characteristicRepository
                    .GetOrCreateTemplateAsync(value.Name, categoryId, value.Unit);

                characteristicGroup.AddCharacteristic(value.Value, template.Id);
            }
        }

        public async Task DeleteProductAsync(Guid productId, Guid userId)
        {
            await _productRepository.DeleteAsync(productId, userId);
            await _productUnitOfWork.SaveChangesAsync();
            _logger.LogInformation("Product with ID {ProductId} successfully deleted", productId);
        }
    }
}
