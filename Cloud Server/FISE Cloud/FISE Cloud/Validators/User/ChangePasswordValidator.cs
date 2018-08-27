using FISE_Cloud.Models;
using FluentValidation;

namespace FISE_Cloud.Validators.User
{
    public class ChangePasswordValidator : BaseValidator<ChangePasswordModel>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.OldPassword).NotEmpty().WithMessage(Resource.ChangePasswordValidator_OldPwdReq);
            
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage(Resource.UserRegistration_PasswordReqError)
                .Matches("(?=.*[a-zA-Z])(?=.*[0-9])(?=.*?[!@#$%^&*()])(?!.* )(?!.*?[></?';:}{\\|\"`~]).{8,100}")
                .WithMessage(Resource.UserRegistration_PwdValidator);
            RuleFor(x => x.NewPassword).NotEqual(x => x.OldPassword).WithMessage(Resource.ChangePasswordValidator_OldandNew_CannotMatch);
            RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage(Resource.ChangePasswordValidator_ConfirmPwdReq);
            RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessage(Resource.ChangePasswordValidator_New_Confirm_Match);
            RuleFor(x => x.ConfirmNewPassword).NotEqual(x => x.OldPassword).WithMessage(Resource.ChangePasswordValidator_OldandNew_CannotMatch);
        }
    }
}