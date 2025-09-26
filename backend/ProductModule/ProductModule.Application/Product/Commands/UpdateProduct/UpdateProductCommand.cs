using MediatR;
using ProductModule.Application.Dtos;

namespace ProductModule.Application.Product.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest
    {
        public UpdateProductCommand(Guid productId, UpdateProductRequest request)
        {
            ProductId = productId;
            Request = request;
        }
        public Guid ProductId { get; set; }
        public UpdateProductRequest Request { get; set; }
    }
}
