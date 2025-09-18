using SharedKernel.Interfaces;

namespace SharedKernel.Events
{
    public class DeleteCategoryEvent : IDomainEvent
    {
        public int CategoryId { get; }
        public DeleteCategoryEvent(int categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
