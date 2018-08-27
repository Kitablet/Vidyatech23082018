using FISE_Cloud.Models;
using FluentValidation;

namespace FISE_Cloud.Validators.User
{
    public class PasswordRecoveryConfirmValidator : BaseValidator<PasswordRecoveryConfirmModel>
    {
        public PasswordRecoveryConfirmValidator()
        {            
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage(Resource.PasswordRecoveryConfirmValidator_NewPwdReq)
                .Matches("(?=.*[a-zA-Z])(?=.*[0-9])(?=.*?[!@#$%^&*()])(?!.* )(?!.*?[></?';:}{\\|\"`~]).{8,100}")
                .WithMessage(Resource.UserRegistration_PwdValidator);
            RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage(Resource.PasswordRecoveryConfirmValidator_ConfirmPwdReq);
            RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessage(Resource.PasswordRecoveryConfirmValidator_New_Confirm_Match);
        }
    }

    public class PasswordRecoveryConfirmStudentValidator : BaseValidator<PasswordRecoveryConfirmModel>
    {
        public PasswordRecoveryConfirmStudentValidator()
        {
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage(Resource.PasswordRecoveryConfirmValidator_NewPwdReq)
                .Length(6, 100).WithMessage(Resource.StudentRegistration_PwdValidator_Msg);
            RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage(Resource.PasswordRecoveryConfirmValidator_ConfirmPwdReq);
            RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessage(Resource.PasswordRecoveryConfirmValidator_New_Confirm_Match);
        }
    }
}