using FISE_Cloud.Models.School;
using FluentValidation;
using System;

namespace FISE_Cloud.Validators.Student
{
    public class ImportStudentValidator : BaseValidator<StudentImportExport>
    {
        public ImportStudentValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(Resource.Student_EditStudent_FirstNameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Firstname_LengthandnospecialcharValidation);
            RuleFor(x => x.LastName)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Lastname_LengthandnospecialcharValidation)
                .When(x => !string.IsNullOrEmpty(x.LastName));
            RuleFor(x => x.Status).NotEqual(false).WithMessage(Resource.Student_GenderValidation);
            RuleFor(x => x.Grade).NotEmpty().WithMessage(Resource.Student_EditStudent_GradeReqError);

            RuleFor(x => x.RollNo)
                .Matches("(^[ A-Za-z0-9]{1,255}$)").WithMessage(Resource.RollNo_Validation)
                .When(x => !string.IsNullOrEmpty(x.RollNo));

            RuleFor(x => x.SubSection)
                .Matches("(^[ A-Za-z0-9.']{1,255}$)").WithMessage(Resource.Section_Validation)
                .When(x => !string.IsNullOrEmpty(x.SubSection));

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Now.Date).WithMessage(Resource.Importstudent_DobValidation)
                .When(x => x.DateOfBirth != null);
            //--
            RuleFor(x => x.TotalFailed).Empty().WithMessage(Resource.UserRegistration_DoBReqError)
                .When(x => x.DateOfBirth == null);

            RuleFor(x => x.ParentEmail).NotEmpty().WithMessage(Resource.UserCreation_EmailReqError).
                EmailAddress().WithMessage(Resource.UserCreation_InvalidEmail)
                .Length(1, 100).WithMessage(Resource.User_EmailLength);

            RuleFor(x => x.ParentMobileNumber).NotEmpty().WithMessage(Resource.UserCreation_MobileNoReqError)
                .Length(10)
                .WithMessage(Resource.UserCreation_InvalidMobileNo);

            RuleFor(x => x.SubscriptionStartDate).NotEmpty().WithMessage(Resource.Student_EditStudent_SubscriptionStartDateReqError);

            RuleFor(x => x.SubscriptionEndDate).NotEmpty().WithMessage(Resource.Student_EditStudent_SubscriptionEndDateReqError)
                .When(x => !x.SubscriptionStartDate.HasValue).WithMessage(Resource.Student_EditStudent_SubscriptionEndDateReqError);

            RuleFor(m => m.SubscriptionEndDate)
            .NotEmpty().WithMessage(Resource.Student_EditStudent_SubscriptionEndDateReqError)
            .GreaterThan(m => m.SubscriptionStartDate.Value)
                            .WithMessage(Resource.Student_Importstudent_SubscriptionEndDate)
            .When(m => m.SubscriptionStartDate.HasValue);
            RuleFor(x => x.SNO).NotEqual(0).WithMessage(Resource.SNO_Required);
        }
    }

    public class ImportChildValidator : BaseValidator<ChildImport>
    {
        public ImportChildValidator()
        {
            RuleFor(x => x.ParentFirstName).NotEmpty().WithMessage(Resource.Parent_Import_FirstNameReqError)
               .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Firstname_LengthandnospecialcharValidation);
            RuleFor(x => x.ParentLastName).NotEmpty().WithMessage(Resource.Parent_Import_LastNameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Lastname_LengthandnospecialcharValidation);

            RuleFor(x => x.ParentEmailID).NotEmpty().WithMessage(Resource.UserCreation_EmailReqError).
                EmailAddress().WithMessage(Resource.UserCreation_InvalidEmail)
                .Length(1, 100).WithMessage(Resource.User_EmailLength);

            RuleFor(x => x.ChildFirstName).NotEmpty().WithMessage(Resource.Student_Import_FirstNameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Firstname_LengthandnospecialcharValidation);
            RuleFor(x => x.ChildLastName).NotEmpty().WithMessage(Resource.Student_Import_LastNameReqError)
                .Matches("(^[ A-Za-z0-9.']{1,50}$)").WithMessage(Resource.Lastname_LengthandnospecialcharValidation);
            RuleFor(x => x.Status).NotEqual(false).WithMessage(Resource.Student_GenderValidation);
            RuleFor(x => x.Grade).NotEmpty().WithMessage(Resource.Student_EditStudent_GradeReqError);

           
            //RuleFor(x => x.RollNo)
            //    .Matches("(^[ A-Za-z0-9]{1,255}$)").WithMessage(Resource.RollNo_Validation)
            //    .When(x => !string.IsNullOrEmpty(x.RollNo));

            //RuleFor(x => x.SubSection)
            //    .Matches("(^[ A-Za-z0-9.']{1,255}$)").WithMessage(Resource.Section_Validation)
            //    .When(x => !string.IsNullOrEmpty(x.SubSection));

            //RuleFor(x => x.DateOfBirth)
            //    .LessThan(DateTime.Now.Date).WithMessage(Resource.Importstudent_DobValidation)
            //    .When(x => x.DateOfBirth != null);
            ////--
            //RuleFor(x => x.TotalFailed).Empty().WithMessage(Resource.UserRegistration_DoBReqError)
            //    .When(x => x.DateOfBirth == null);

            

            //RuleFor(x => x.ParentMobileNumber).NotEmpty().WithMessage(Resource.UserCreation_MobileNoReqError)
            //    .Length(10)
            //    .WithMessage(Resource.UserCreation_InvalidMobileNo);

            RuleFor(x => x.SubscriptionStartDate).NotEmpty().WithMessage(Resource.Student_EditStudent_SubscriptionStartDateReqError);

            RuleFor(x => x.SubscriptionEndDate).NotEmpty().WithMessage(Resource.Student_EditStudent_SubscriptionEndDateReqError)
                .When(x => !x.SubscriptionStartDate.HasValue).WithMessage(Resource.Student_EditStudent_SubscriptionEndDateReqError);

            RuleFor(m => m.SubscriptionEndDate)
            .NotEmpty().WithMessage(Resource.Student_EditStudent_SubscriptionEndDateReqError)
            .GreaterThan(m => m.SubscriptionStartDate.Value)
                            .WithMessage(Resource.Student_Importstudent_SubscriptionEndDate)
            .When(m => m.SubscriptionStartDate.HasValue);
            RuleFor(x => x.SNO).NotEqual(0).WithMessage(Resource.SNO_Required);
        }
    }
}