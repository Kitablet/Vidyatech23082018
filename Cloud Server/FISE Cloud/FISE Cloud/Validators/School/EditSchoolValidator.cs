using FluentValidation;


namespace FISE_Cloud.Validators.School
{
    public class EditSchoolValidator : BaseValidator<FISE_Cloud.Models.School.School>
    {
        public EditSchoolValidator()
        {
            RuleFor(x => x.SchoolName).NotEmpty().WithMessage(Resource.CreateSchool_SchoolNameReqError)
                .Matches("(^[ A-Za-z0-9-._',&]{1,255}$)").WithMessage(Resource.CreateSchool_SchoolNameLength);
            RuleFor(x => x.AddressLine1).NotEmpty().WithMessage(Resource.UserRegistration_AddressReqError)
                .Matches("(^[ A-Za-z0-9-._@:',#/&]{1,255}$)").WithMessage(Resource.Address_Length);
            RuleFor(x => x.AddressLine2)
                 .Matches("(^[ A-Za-z0-9-._@:',#/&]{0,255}$)").WithMessage(Resource.Address_Length);
            RuleFor(x => x.City).NotEmpty().WithMessage(Resource.UserRegistration_CityReqError);
            RuleFor(x => x.PinCode).NotEmpty().WithMessage(Resource.UserRegistration_PincodeReqError)
                .Must(x => x <= 999999999 && x >= 100000)
                .WithMessage(Resource.UserCreation_InvalidPincodeError);
            RuleFor(x => x.PrincipalEmail).NotEmpty().WithMessage(Resource.UserCreation_PrincipalEmailReqError)
                .EmailAddress().WithMessage(Resource.UserCreation_InvalidEmail)
                .Length(1, 100).WithMessage(Resource.User_EmailLength);
            RuleFor(x => x.PrincipalName).NotEmpty().WithMessage(Resource.UserRegistration_PrincipalNameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.UserRegistration_PrincipalName_LengthandnospecialcharValidation)
                .Length(1, 50).WithMessage(Resource.UserRegistration_PrincipalName_LengthandnospecialcharValidation);
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(Resource.CreateSchool_PhoneNoReqError)
                .Matches("(^[0-9]{5,15}$)").WithMessage(Resource.CreateSchool_PhoneNoMinMaxLength);          
        }
    }
}