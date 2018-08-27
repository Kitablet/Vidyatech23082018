using FISE_Cloud.Models;
using FluentValidation;

namespace FISE_Cloud.Validators.User
{
    public class EditParentValidator : BaseValidator<UserRegistrationModel>
    {
        public EditParentValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(Resource.Student_EditStudent_FirstNameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Firstname_LengthandnospecialcharValidation)
                .Length(1, 50).WithMessage(Resource.Firstname_LengthandnospecialcharValidation);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(Resource.Student_EditStudent_LastNameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Lastname_LengthandnospecialcharValidation)
                .Length(1, 50).WithMessage(Resource.Lastname_LengthandnospecialcharValidation);

            RuleFor(x => x.MobileNumber).NotEmpty().WithMessage(Resource.UserCreation_MobileNoReqError)
                 .Length(10).WithMessage(Resource.UserCreation_InvalidMobileNo);
            RuleFor(x => x.Gender).NotEmpty().WithMessage(Resource.Student_EditStudent_GenderReqError);


        }
    }
}