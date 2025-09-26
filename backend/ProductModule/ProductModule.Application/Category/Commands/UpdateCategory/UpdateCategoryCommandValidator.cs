using FluentValidation;

namespace ProductModule.Application.Category.Commands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID must be greater than zero.");

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Update request cannot be null.")
                .SetValidator(new UpdateCategoryRequestValidator());
        }
    }
}
