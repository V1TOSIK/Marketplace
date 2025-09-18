using MediatR;

namespace ProductModule.Application.Category.Commands.DeleteCategory
{
    public class DeleteCategoryCommand : IRequest
    {
        public DeleteCategoryCommand(int categoryId)
        {
            CategoryId = categoryId;
        }
        public int CategoryId { get; set; }
    }
}
