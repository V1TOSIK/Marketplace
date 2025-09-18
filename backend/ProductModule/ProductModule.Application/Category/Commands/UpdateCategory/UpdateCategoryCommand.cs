using MediatR;

namespace ProductModule.Application.Category.Commands.UpdateCategory
{
    public class UpdateCategoryCommand : IRequest
    {
        public UpdateCategoryCommand(int categoryId, string name)
        {
            CategoryId = categoryId;
            Name = name;
        }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
