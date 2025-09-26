using MediatR;

namespace ProductModule.Application.Product.Commands.PublishProduct
{
    public class PublishProductCommand : IRequest
    {
        public Guid ProductId { get; set; }

        public PublishProductCommand(Guid productId)
        {
            ProductId = productId;
        }
    }
}
