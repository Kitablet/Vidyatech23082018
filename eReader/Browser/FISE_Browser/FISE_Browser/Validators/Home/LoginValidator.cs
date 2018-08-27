
using FISE_Browser.Models;
using FluentValidation;

namespace FISE_Browser.Validators.Home
{   
    public class UserLoginValidator : BaseValidator<UserBase>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage(Resource.UsernameRequired);
            RuleFor(x => x.Password).NotEmpty().WithMessage(Resource.PasswordRequired);
        }
    }
}