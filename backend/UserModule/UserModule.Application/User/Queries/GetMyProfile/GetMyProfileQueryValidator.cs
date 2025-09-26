using FluentValidation;

namespace UserModule.Application.User.Queries.GetMyProfile
{
    public class GetMyProfileQueryValidator : AbstractValidator<GetMyProfileQuery>
    {
        public GetMyProfileQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required")
                .NotEqual(Guid.Empty).WithMessage("UserId cannot be empty GUID");
        }
    }
}
