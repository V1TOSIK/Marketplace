using FluentValidation;
using SharedKernel.Pagination;

namespace ProductModule.Application.Product.Queries.GetFilteredProducts
{
    public class GetFilteredProductQueryValidator : AbstractValidator<GetFilteredProductQuery>
    {
        public GetFilteredProductQueryValidator()
        {
            Include(new PaginationRequestValidator());

            RuleForEach(x => x.Characteristics)
                .SetValidator(new CharacteristicFilterValidator());

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0).WithMessage("MinPrice must be greater than or equal to 0.")
                .When(x => x.MinPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0).WithMessage("MaxPrice must be greater than or equal to 0.")
                .When(x => x.MaxPrice.HasValue);

            RuleFor(x => x.Location)
                .ForEach(locationRule => locationRule
                    .NotEmpty().WithMessage("Each location must not be empty.")
                    .MaximumLength(100).WithMessage("Each location must not exceed 100 characters."))
                .When(x => x.Location != null && x.Location.Any());

            RuleFor(x => x.SortedBy)
                .MaximumLength(50).WithMessage("SortedBy must not exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.SortedBy));

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.")
                .When(x => x.CategoryId.HasValue);

            RuleFor(x => x.SortDescending)
                .NotNull().WithMessage("SortDescending must be specified.");
        }
    }
}
