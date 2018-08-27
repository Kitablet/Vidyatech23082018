using FISE_Cloud.Models;
using FluentValidation;


namespace FISE_Cloud.Validators.User
{
    public class UsernameRecoveryValidator : BaseValidator<UsernameRecoveryModel>
    {
        public UsernameRecoveryValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Resource.UserCreation_EmailReqError)
                .EmailAddress().WithMessage(Resource.UserCreation_InvalidEmail)
                .Length(1, 100).WithMessage(Resource.User_EmailLength);
        }

    }
}