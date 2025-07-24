using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Dtos.Requests;
using ProductModule.Application.Dtos.Responses;
using ProductModule.Application.Interfaces;
using ProductModule.Domain.Entities;
using ProductModule.Domain.Enums;
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

        public async Task<IEnumerable<ProductResponse>> GetProductsByFilterAsync(ProductFilterRequest filter)
        {
            var query = _productRepository.Query();

            query = query.Where(p => p.Status == Status.Published);
            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price.Amount <= filter.MaxPrice);
            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price.Amount >= filter.MinPrice);

            if (filter.Location?.Any() == true)
                query = query.Where(p => filter.Location.Contains(p.Location));

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId);

            if (filter.Characteristics?.Any() == true)
            {
                var filters = filter.Characteristics
                    .Select(c => (c.TemplateId, c.Values))
                    .ToList();
                var productIds = await _productRepository
                    .GetProductIdsFilteredByCharacteristics(filters);
                query = query.Where(p => productIds.Contains(p.Id));
            }

            var result = await query.ToListAsync();

            return result.Select(p => new ProductResponse
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

        public async Task<IEnumerable<ProductResponse>> GetMyProducts(Guid userId)
        {
            var products = await _productRepository.GetByUserIdWithDraftsAsync(userId);
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

        public async Task PublishProductAsync(Guid productId, Guid userId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", productId);
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }
            if (product.UserId != userId)
            {
                _logger.LogWarning("User {UserId} is not authorized to publish product {ProductId}", userId, productId);
                throw new UnauthorizedAccessException("You are not authorized to publish this product.");
            }
            product.Publish();
            await _productUnitOfWork.SaveChangesAsync();
            _logger.LogInformation("Product '{ProductName}' (ID: {ProductId}) successfully published", product.Name, product.Id);
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
                request.CategoryId,
                Status.Draft.ToString());

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
