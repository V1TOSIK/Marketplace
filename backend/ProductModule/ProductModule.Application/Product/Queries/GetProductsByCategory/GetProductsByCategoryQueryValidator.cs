using FluentValidation;
using ProductModule.Application.Product.Queries.GetProductByCategory;
using SharedKernel.Pagination;

namespace ProductModule.Application.Product.Queries.GetProductsByCategory
{
    public class GetProductsByCategoryQueryValidator : AbstractValidator<GetProductsByCategoryQuery>
    {
        public GetProductsByCategoryQueryValidator()
        {
            Include(new PaginationRequestValidator());

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");
        }
    }
}
