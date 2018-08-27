using FISE_Cloud.Models;
using FluentValidation;

namespace FISE_Cloud.Validators.User
{
    public class PasswordRecoveryValidator : BaseValidator<PasswordRecoveryModel>
    {
        public PasswordRecoveryValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage(Resource.PasswordRecoveryValidator_UserNameReq)
                .Length(5, 25).WithMessage(Resource.PasswordRecoveryValidator_UsernameLength);
        }

    }
}
    