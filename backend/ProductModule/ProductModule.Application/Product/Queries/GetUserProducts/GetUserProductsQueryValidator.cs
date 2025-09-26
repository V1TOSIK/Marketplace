using FluentValidation;
using SharedKernel.Pagination;

namespace ProductModule.Application.Product.Queries.GetUserProducts
{
    public class GetUserProductsQueryValidator : AbstractValidator<GetUserProductsQuery>
    {
        public GetUserProductsQueryValidator()
        {
            Include(new PaginationRequestValidator());

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
