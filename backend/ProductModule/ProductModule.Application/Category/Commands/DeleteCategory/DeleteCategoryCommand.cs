using MediatR;

namespace ProductModule.Application.Category.Commands.DeleteCategory
{
    public class DeleteCategoryCommand : IRequest
    {
        public int CategoryId { get; set; }
        
        public DeleteCategoryCommand(int categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
