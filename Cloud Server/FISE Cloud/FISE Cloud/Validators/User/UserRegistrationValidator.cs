using FISE_Cloud.Models;
using FluentValidation;

namespace FISE_Cloud.Validators.User
{
    public class UserRegistrationValidator : BaseValidator<UserRegistrationModel>
    {
        public UserRegistrationValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage(Resource.UserRegistration_UsernameReqError)
                .Matches("(^[a-zA-Z0-9!#$%&'*+-/=?^_`{|}~@]+([.][a-zA-Z0-9!#$%&'*+-/=?^_`{|}~@]+)*$)").WithMessage(Resource.UserRegistration_UsernameLength)
                .Length(5, 25).WithMessage(Resource.UserRegistration_UsernameLength);
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(Resource.UserRegistration_FirstnameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Firstname_LengthandnospecialcharValidation)
                .Length(1, 50).WithMessage(Resource.Firstname_LengthandnospecialcharValidation);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(Resource.UserRegistration_LastnameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Lastname_LengthandnospecialcharValidation)
                .Length(1, 50).WithMessage(Resource.Lastname_LengthandnospecialcharValidation);
            RuleFor(x => x.Password).NotEmpty().WithMessage(Resource.UserRegistration_PasswordReqError)
                .Matches("(?=.*[a-zA-Z])(?=.*[0-9])(?=.*?[!@#$%^&*()])(?!.* )(?!.*?[></?';:}{\\|\"`~]).{8,100}")
                .WithMessage(Resource.UserRegistration_PwdValidator);
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(Resource.UserRegistration_ConfirmPasswordReqError);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(Resource.UserRegistration_Password_Confirm_Match);
            RuleFor(x => x.MobileNumber).NotEmpty().WithMessage(Resource.UserCreation_MobileNoReqError)
                .Length(10).WithMessage(Resource.UserCreation_InvalidMobileNo);
            RuleFor(x => x.Gender).NotEmpty().WithMessage(Resource.UserCreation_GenderReqError);
            RuleFor(x => x.IsTermAndConditionAccepted).Equal(true).WithMessage(Resource.TermAndConditionError);
        }
    }
}