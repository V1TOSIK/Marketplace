using FluentValidation;

namespace ProductModule.Application.Product.Commands.UpdateProduct.GroupUpdates
{
    public class GroupCreateDtoValidator : AbstractValidator<GroupCreateDto>
    {
        public GroupCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must be shorter than 100 symbols");
        }
    }
}
