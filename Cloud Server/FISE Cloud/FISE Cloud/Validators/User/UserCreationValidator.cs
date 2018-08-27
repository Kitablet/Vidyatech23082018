using FISE_Cloud.Models;
using FluentValidation;

namespace FISE_Cloud.Validators.User
{
    public class UserCreationValidator: BaseValidator<UserCreationModel>
    {
        public UserCreationValidator ()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Resource.UserCreation_EmailReqError)
                .EmailAddress().WithMessage(Resource.UserCreation_InvalidEmail)
                .Length(1, 100).WithMessage(Resource.User_EmailLength);
            RuleFor(x => x.MobileNo).NotEmpty().WithMessage(Resource.UserCreation_MobileNoReqError)
                .Length(10)
                .WithMessage(Resource.UserCreation_InvalidMobileNo);
        }
    }
    public class GetRegistrationValidator : BaseValidator<UserCreationModel>
    {
        public GetRegistrationValidator(int length)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Resource.UserCreation_EmailReqError)
                .EmailAddress().WithMessage(Resource.UserCreation_InvalidEmail)
                .Length(1, 100).WithMessage(Resource.User_EmailLength);
            RuleFor(x => x.MobileNo).NotEmpty().WithMessage(Resource.UserCreation_MobileNoReqError)
                .Length(length)
                .WithMessage(Resource.UserValidation_Mobileno_6digit_Error.Replace("#",length.ToString()));
        }
    }

    public class UserUpdateValidator : BaseValidator<UserRegistrationModel>
    {
        public UserUpdateValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Resource.UserCreation_EmailReqError)
                .EmailAddress().WithMessage(Resource.UserCreation_InvalidEmail)
                .Length(1, 100).WithMessage(Resource.User_EmailLength);
            RuleFor(x => x.MobileNumber).NotEmpty().WithMessage(Resource.UserCreation_MobileNoReqError)
                .Length(10)
                .WithMessage(Resource.UserCreation_InvalidMobileNo);
        }
    }
}