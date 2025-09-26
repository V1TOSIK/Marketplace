using MediatR;

namespace ProductModule.Application.Category.Commands.UpdateCategory
{
    public class UpdateCategoryCommand : IRequest
    {
        public UpdateCategoryCommand(int categoryId, UpdateCategoryRequest request)
        {
            CategoryId = categoryId;
            Request = request;
        }
        public int CategoryId { get; set; }
        public UpdateCategoryRequest Request { get; set; }
    }
}
