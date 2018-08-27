using System;
using System.Collections.Generic;

namespace FISE_API.Models
{
    public class UserBase : ReturnStatus
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Type { get; set; }
    }
    public class UserCommon : UserBase
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public bool Status { get; set; }
        public bool IsTrashed { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string AvatarImage { get; set; }
        public int DobDate { get; set; }
        public int DobMonth { get; set; }
        public int DobYear { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
    public class User : UserCommon
    {
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int PinCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Nationality { get; set; }
        public string City { get; set; }
    }
    public class StudentModel : User
    {
        public string Grade { get; set; }
        public string SubSection { get; set; }
        public string RollNo { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionEndDate { get; set; }
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string ParentEmail { get; set; }
        public string ParentMobileNo { get; set; }
        public string HomeDevices { get; set; }
        public string SchoolUId { get; set; }
        public string Result { get; set; }
        public bool SchoolStatus { get; set; }
        public string ParentUsername { get; set; }
        public string ParentFirstname { get; set; }
        public string ParentLastname { get; set; }
    }

    public class StudentRegistrationModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
    }

    public class UserProfile
    {
        public User User { get; set; }
    }

    public class UserPasswordRecovery
    {
        public UserPasswordRecovery() {
            Users = new List<PasswordRecoveryUsers>();
        }
        public UserStatus Status { get; set; }
        public List<PasswordRecoveryUsers> Users { get; set; }
    }
    public class PasswordRecoveryUsers
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
}