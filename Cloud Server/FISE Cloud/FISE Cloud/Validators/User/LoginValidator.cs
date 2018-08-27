using FISE_Cloud.Models;
using FluentValidation;

namespace FISE_Cloud.Validators.User
{
    public class LoginValidator: BaseValidator<LoginModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage(Resource.PopupLogin_UsernameReqError);
            RuleFor(x => x.Password).NotEmpty().WithMessage(Resource.PopupLogin_PasswordReqError);
        }
    }
}