using FluentValidation;

namespace ProductModule.Application.Product.Commands.UpdateProduct.GroupUpdates
{
    public class GroupBatchDtoValidator : AbstractValidator<GroupBatchDto>
    {
        public GroupBatchDtoValidator()
        {
            RuleForEach(x => x.Added)
                .SetValidator(new GroupCreateDtoValidator());

            RuleForEach(x => x.Updated)
                .SetValidator(new GroupUpdateDtoValidator());
        }
    }
}
