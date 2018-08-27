using System;

namespace Kitablet
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string RegistrationDate { get; set; }
        public string Role { get; set; }
        public string SchoolName { get; set; }
        public string Grade { get; set; }
        public string SubSection { get; set; }
        public int GradeId { get; set; }
        public int SubSectionId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarImage { get; set; }
        public int AvatarId { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string City { get; set; }
    }

    public class LoginObject
    {
        public int Status { get; set; }
        public User User { get; set; }
    }
}
