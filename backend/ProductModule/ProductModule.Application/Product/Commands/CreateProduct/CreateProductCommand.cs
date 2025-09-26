using MediatR;
using ProductModule.Application.Dtos;

namespace ProductModule.Application.Product.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public Guid UserId { get; }
        public CreateProductRequest Request { get; }

        public CreateProductCommand(Guid userId, CreateProductRequest request)
        {
            UserId = userId;
            Request = request;
        }
    }
}
