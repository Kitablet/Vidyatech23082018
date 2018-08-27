using FISE_Cloud.Models;
using FluentValidation;

namespace FISE_Cloud.Validators.Student
{
    public class EditStudentValidator : BaseValidator<StudentRegistrationModel>
    {
        public EditStudentValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(Resource.Student_EditStudent_FirstNameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Firstname_LengthandnospecialcharValidation)
                .Length(1, 50).WithMessage(Resource.Firstname_LengthandnospecialcharValidation);
           
            When(x => !string.IsNullOrEmpty(x.LastName), () =>
                {
                    RuleFor(x => x.LastName).NotEmpty().WithMessage(Resource.Student_EditStudent_LastNameReqError)
                        .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Lastname_LengthandnospecialcharValidation)
                        .Length(1, 50).WithMessage(Resource.Lastname_LengthandnospecialcharValidation);
                });
            
            RuleFor(x => x.Grade).NotEmpty().WithMessage(Resource.Student_EditStudent_GradeReqError);
            
            When(x => !string.IsNullOrEmpty(x.SubSection), () =>
            {
                RuleFor(x => x.SubSection).NotEmpty().WithMessage(Resource.Student_EditStudent_SectionReqError)
                    .Matches("(^[ A-Za-z0-9.']{1,255}$)").WithMessage(Resource.Section_Validation)
                    .Length(1, 255).WithMessage(Resource.Section_Validation);
            });
            
            RuleFor(x => x.DobDate).NotEmpty().When(x => x.DobMonth > 0 || x.DobYear > 0).WithMessage(Resource.UserRegistrationdate);
            RuleFor(x => x.DobMonth).NotEmpty().When(x => x.DobDate > 0 || x.DobYear > 0).WithMessage(Resource.UserRegistrationmonth);
            RuleFor(x => x.DobYear).NotEmpty().When(x => x.DobMonth > 0 || x.DobDate > 0).WithMessage(Resource.UserRegistrationyear);
            
        }
    }
}