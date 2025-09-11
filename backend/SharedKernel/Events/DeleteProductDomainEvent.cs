using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class DeleteProductDomainEvent : IDomainEvent
    {
        public Guid ProductId { get; private set; }
        public DeleteProductDomainEvent(Guid productId)
        {
            ProductId = productId;
        }
    }
}
