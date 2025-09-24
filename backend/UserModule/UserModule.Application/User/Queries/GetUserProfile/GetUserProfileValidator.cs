using FluentValidation;

namespace UserModule.Application.User.Queries.GetUserProfile
{
    public class GetUserProfileValidator : AbstractValidator<GetUserProfileQuery>
    {
        public GetUserProfileValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required")
                .NotEqual(Guid.Empty).WithMessage("UserId cannot be empty GUID");
        }
    }
}
