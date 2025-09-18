using MediatR;

namespace ProductModule.Application.Category.Commands.AddCategory
{
    public class AddCategoryCommand : IRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
