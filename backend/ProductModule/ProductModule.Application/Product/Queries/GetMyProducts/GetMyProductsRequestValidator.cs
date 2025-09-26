using FluentValidation;
using SharedKernel.Pagination;

namespace ProductModule.Application.Product.Queries.GetMyProducts
{
    public class GetMyProductsRequestValidator : AbstractValidator<GetMyProductsRequest>
    {
        public GetMyProductsRequestValidator()
        {
            Include(new PaginationRequestValidator());

            RuleForEach(x => x.Statuses)
                .MaximumLength(50).WithMessage("Status must not exceed 50 characters.")
                .When(x => x.Statuses != null && x.Statuses.Any());
        }
    }
}
