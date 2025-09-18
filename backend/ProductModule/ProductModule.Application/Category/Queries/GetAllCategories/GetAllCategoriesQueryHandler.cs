using MediatR;
using Microsoft.Extensions.Logging;
using ProductModule.Application.Dtos;
using ProductModule.Application.Interfaces.Repositories;

namespace ProductModule.Application.Category.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<GetAllCategoriesQueryHandler> _logger;

        public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository,
            ILogger<GetAllCategoriesQueryHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }
        public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetAllAsync(cancellationToken);
        }
    }
}
