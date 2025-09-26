using FluentValidation;

namespace ProductModule.Application.Category.Commands.UpdateCategory
{
    public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");
        }
    }
}
