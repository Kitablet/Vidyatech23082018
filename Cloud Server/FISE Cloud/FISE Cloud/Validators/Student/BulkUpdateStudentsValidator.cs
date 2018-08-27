using FISE_Cloud.Models.School;
using FluentValidation;

namespace FISE_Cloud.Validators.Student
{
    public class BulkUpdateStudentsValidator : BaseValidator<StudentImportExport>
    {
        public BulkUpdateStudentsValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(Resource.Student_EditStudent_FirstNameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Firstname_LengthandnospecialcharValidation);
            RuleFor(x => x.LastName)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Lastname_LengthandnospecialcharValidation)
                .When(x => !string.IsNullOrEmpty(x.LastName)); 
            RuleFor(x => x.Grade).NotEmpty().WithMessage(Resource.Student_EditStudent_GradeReqError);
            
            RuleFor(x => x.RollNo)
                .Matches("(^[ A-Za-z0-9]{1,255}$)").WithMessage(Resource.RollNo_Validation)
                .When(x=>!string.IsNullOrEmpty(x.RollNo)); 
            
            RuleFor(x => x.SubSection)
                .Matches("(^[ A-Za-z0-9.']{1,255}$)").WithMessage(Resource.Section_Validation)
                .When(x => !string.IsNullOrEmpty(x.SubSection)); 

            When(x => x.Status, () =>
            {
                RuleFor(x => x.SubscriptionStartDate).NotEmpty().WithMessage(Resource.Student_EditStudent_SubscriptionStartDateReqError);
                  
                RuleFor(x => x.SubscriptionEndDate).NotEmpty().WithMessage(Resource.Student_EditStudent_SubscriptionEndDateReqError)
                .When(x => !x.SubscriptionStartDate.HasValue).WithMessage(Resource.Student_EditStudent_SubscriptionEndDateReqError);

                RuleFor(m => m.SubscriptionEndDate)
                .NotEmpty().WithMessage(Resource.Student_EditStudent_SubscriptionEndDateReqError)
                .GreaterThan(m => m.SubscriptionStartDate.Value)
                                .WithMessage(Resource.Student_Importstudent_SubscriptionEndDate)
                .When(m => m.SubscriptionStartDate.HasValue);
            });
            RuleFor(x => x.IsRenew).NotEmpty().WithMessage(Resource.IsRenew_Required);
            RuleFor(x => x.UniqueId).NotEmpty().WithMessage(Resource.UniqueId_Required)
                .Matches("^([a-zA-Z0-9]{8,8})-([a-zA-Z0-9]{4,4})-([a-zA-Z0-9]{4,4})-([a-zA-Z0-9]{4,4})-([a-zA-Z0-9]{12,12}){1,36}$")
                .WithMessage(Resource.UniqueId_Length);
            RuleFor(x => x.SNO).NotEqual(0).WithMessage(Resource.SNO_Required);
        }
    }
}