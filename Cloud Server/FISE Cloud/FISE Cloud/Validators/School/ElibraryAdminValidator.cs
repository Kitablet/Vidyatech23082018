using FluentValidation;
using FISE_Cloud.Models;

namespace FISE_Cloud.Validators.School
{
    public class ElibraryAdminValidator : BaseValidator<ElibraryAdminRegistrationModel>
    {
        public ElibraryAdminValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(Resource.UserRegistration_FirstnameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Firstname_LengthandnospecialcharValidation)
                .Length(1, 50).WithMessage(Resource.Firstname_LengthandnospecialcharValidation);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(Resource.UserRegistration_LastnameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Lastname_LengthandnospecialcharValidation)
                .Length(1, 50).WithMessage(Resource.Lastname_LengthandnospecialcharValidation);
            RuleFor(x => x.MobileNumber).NotEmpty().WithMessage(Resource.UserCreation_MobileNoReqError)
                .Length(10)
                .WithMessage(Resource.UserCreation_InvalidMobileNo);
            RuleFor(x => x.Gender).NotEmpty().WithMessage(Resource.Student_EditStudent_GenderReqError);
        }
    }
}