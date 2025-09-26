using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Interfaces;
using ProductModule.Application.Interfaces.Repositories;
using ProductModule.Application.Product.Commands.UpdateProduct.CharacteristicUpdates;
using ProductModule.Application.Product.Commands.UpdateProduct.GroupUpdates;
using ProductModule.Domain.Entities;
using ProductModule.Domain.Enums;
using ProductModule.Domain.Exceptions;

namespace ProductModule.Application.Product.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICharacteristicRepository _characteristicRepository;
        private readonly IProductUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(IProductRepository productRepository,
            ICharacteristicRepository characteristicRepository,
            IProductUnitOfWork unitOfWork,
            ILogger<UpdateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _characteristicRepository = characteristicRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdWithCharacteristicsAsync(command.ProductId, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found.", command.ProductId);
                throw new NullableProductException("Product not found.");
            }
            if (product.Status != Status.Draft)
            {
                _logger.LogWarning("Attempt to update product with status not draft with ID {ProductId}.", command.ProductId);
                throw new InvalidProductOperationException("Cannot update a product with not draft status.");
            }
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                product.UpdateProduct(command.Request.Name,
                    command.Request.PriceAmount,
                    command.Request.PriceCurrency,
                    command.Request.Location,
                    command.Request.Description,
                    command.Request.CategoryId);
                await SyncProduct(product, command.Request.Characteristics, command.Request.Groups, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            _logger.LogInformation("Product with ID {ProductId} updated successfully.", command.ProductId);
        }

        public async Task SyncProduct(Domain.Entities.Product product, CharacteristicBatchDto characteristics, GroupBatchDto groups, CancellationToken cancellationToken)
        {
            foreach (var groupId in groups.Deleted ?? [])
            {
                product.RemoveCharacteristicGroup(groupId);
            }

            foreach (var g in groups.Updated ?? [])
            {
                product.UpdateCharacteristicGroup(g.Id, g.Name);
            }

            foreach (var g in groups.Added ?? [])
            {
                product.AddCharacteristicGroup(new CharacteristicGroup(g.Name, product.Id));
            }

            // 🔹 Process characteristics
            foreach (var charId in characteristics.Deleted ?? [])
            {
                product.RemoveCharacteristic(charId);
            }

            foreach (var c in characteristics.Updated ?? [])
            {
                product.UpdateCharacteristic(c.Id, c.Value);
            }

            foreach (var c in characteristics.Added ?? [])
            {
                Func<Task<int>> getTemplateId = async () =>
                {
                    var template = await _characteristicRepository.GetTemplateByPropertiesAsync(c.Name, c.Unit ?? "", cancellationToken);
                    if (template == null)
                        throw new NullableCharacteristicTemplateException($"Template for {c.Name} ({c.Unit}) not found.");

                    return template.Id;
                };
                await product.AddCharacteristic(c.GroupId, c.Value, getTemplateId);

            }
        }
    }
}
