using MediatR;
using ProductModule.Application.Dtos;

namespace ProductModule.Application.Category.Queries.GetAllCategories
{
    public class GetAllCategoriesQuery : IRequest<List<CategoryDto>>
    {
    }
}
