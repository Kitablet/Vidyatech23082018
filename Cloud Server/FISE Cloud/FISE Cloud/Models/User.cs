using System;
using System.ComponentModel.DataAnnotations;

namespace FISE_Cloud.Models
{
    public class User : ReturnStatus
    {
        public int UserId { get; set; }
        public string Email { get; set; }

        public string Username { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Display(Name = "Registration Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? RegistrationDate { get; set; }
        [Display(Name = "Creation Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime CreationDate { get; set; }

        public string Role { get; set; }

        public string Token { get; set; }
        [Display(Name = "School ID")]
        public int SchoolId { get; set; }
        [Display(Name = "School Name")]
        public string SchoolName { get; set; }
        public bool Status { get; set; }
        public bool IsTrashed { get; set; }
        public string Grade { get; set; }
        public string SubSection { get; set; }
        public string AvatarImage { get; set; }
        public int AvatarId { get; set; }
        public string Type { get; set; }
        public string SchoolUId { get; set; }
        public bool IsInvalidToken { get; set; }
    }
    public class UserRegistrationModel : User
    {
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string PasswordSalt { get; set; }

        [Display(Name = "Address Line1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line2")]
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        [Display(Name = "Pin Code")]
        //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Please enter only digits")]
        public int PinCode { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }
        public int? DobDate { get; set; }
        public int? DobMonth { get; set; }
        public int? DobYear { get; set; }
        public string Result { get; set; }
        public bool IsTermAndConditionAccepted { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
    public class StudentRegistrationModel : UserRegistrationModel
    {
        [Display(Name = "Roll No.")]
        public string RollNo { get; set; }
        [Display(Name = "Parent Email")]
        public string ParentEmail { get; set; }
        [Display(Name = "Parent Mobile Number")]
        public string ParentMobileNo { get; set; }
        [Display(Name = "Subscription Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? SubscriptionStartDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "Subscription End Date")]
        public DateTime? SubscriptionEndDate { get; set; }
        public string HomeDevices { get; set; }
        public bool SchoolStatus { get; set; }
        [Display(Name = "Parent Username")]
        public string ParentUsername { get; set; }
        public string ParentFirstname { get; set; }
        public string ParentLastname { get; set; }
    }

    public class ElibraryAdminRegistrationModel : UserRegistrationModel
    {
        public string School { get; set; }
    }

    public class UserProfile
    {
        public int Status { get; set; }
        public UserRegistrationModel User { get; set; }
    }

    public class StudentProfile : StudentRegistrationModel
    {
        public string Grades { set; get; }
        public int APIStatus { get; set; }
    }
}