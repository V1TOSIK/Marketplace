using FluentValidation;

namespace ProductModule.Application.Product.Queries.GetMyProducts
{
    public class GetMyProductsQueryValidator : AbstractValidator<GetMyProductsQuery>
    {
        public GetMyProductsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request cannot be null.")
                .SetValidator(new GetMyProductsRequestValidator());
        }
    }
}
