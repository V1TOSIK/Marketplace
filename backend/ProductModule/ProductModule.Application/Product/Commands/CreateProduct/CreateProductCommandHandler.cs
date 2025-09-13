using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Domain.Entities;
using ProductModule.Domain.Enums;
using ProductModule.Application.Interfaces.Repositories;
using ProductModule.Application.Interfaces;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;

namespace ProductModule.Application.Product.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductUnitOfWork _productUnitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICharacteristicRepository _characteristicRepository;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(IProductRepository productRepository,
            IProductUnitOfWork productUnitOfWork,
            ICurrentUserService currentUserService,
            ICharacteristicRepository characteristicRepository,
            ILogger<CreateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _productUnitOfWork = productUnitOfWork;
            _currentUserService = currentUserService;
            _characteristicRepository = characteristicRepository;
            _logger = logger;
        }
        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                _logger.LogWarning("[Product Module] User ID is empty. Cannot create product.");
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var product = Domain.Entities.Product.Create(
                userId.Value,
                request.Name,
                request.PriceAmount,
                request.PriceCurrency,
                request.Location,
                request.Description,
                request.CategoryId,
                Status.Draft.ToString());

            if (request.CharacteristicGroups?.Any() == true)
            {
                foreach (var group in request.CharacteristicGroups)
                {
                    var characteristicGroup = CharacteristicGroup.Create(group.Name, product.Id);
                    await AddCharacteristics(
                        characteristicGroup,
                        group.Characteristics,
                        request.CategoryId,
                        cancellationToken);

                    product.AddCharacteristicGroup(characteristicGroup);
                }
            }

            await _productRepository.AddAsync(product, cancellationToken);
            await _productUnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("[Product Module] Product '{ProductName}' (ID: {ProductId}) successfully created", product.Name, product.Id);
            return product.Id;
        }

        private async Task AddCharacteristics(
            CharacteristicGroup characteristicGroup,
            IEnumerable<CharacteristicDto>? characteristics,
            int categoryId,
            CancellationToken cancellationToken)
        {
            if (!(characteristics?.Any() ?? false))
                return;

            foreach (var value in characteristics)
            {
                var template = await _characteristicRepository
                    .GetOrCreateTemplateAsync(value.Name, categoryId, value.Unit ?? "", cancellationToken);

                characteristicGroup.AddCharacteristic(value.Value, template.Id);
            }
        }
    }
}
