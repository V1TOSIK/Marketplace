using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class DeleteCategoryDomainEvent : IDomainEvent
    {
        public int CategoryId { get; }
        public DeleteCategoryDomainEvent(int categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
