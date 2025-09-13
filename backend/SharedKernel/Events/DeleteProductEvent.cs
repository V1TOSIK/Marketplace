using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class DeleteProductEvent : IDomainEvent
    {
        public Guid ProductId { get; private set; }
        public DeleteProductEvent(Guid productId)
        {
            ProductId = productId;
        }
    }
}
