using FISE_Cloud.Models;
using FluentValidation;

namespace FISE_Cloud.Validators.User
{
    public class StudentRegistrationValidator:BaseValidator<StudentRegistrationModel>
    {
        public StudentRegistrationValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage(Resource.UserRegistration_UsernameReqError)
               .Matches("(^[a-zA-Z0-9!#$%&'*+-/=?^_`{|}~@]+([.][a-zA-Z0-9!#$%&'*+-/=?^_`{|}~@]+)*$)").WithMessage(Resource.UserRegistration_UsernameLength)
                .Length(5,25).WithMessage(Resource.UserRegistration_UsernameLength);
            RuleFor(x => x.Password).NotEmpty().WithMessage(Resource.UserRegistration_PasswordReqError)
                .Length(6, 100).WithMessage(Resource.StudentRegistration_PwdValidator_Msg);
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(Resource.UserRegistration_ConfirmPasswordReqError);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(Resource.UserRegistration_Password_Confirm_Match);
            RuleFor(x => x.IsTermAndConditionAccepted).Equal(true).WithMessage(Resource.TermAndConditionError);
        }
    }
}