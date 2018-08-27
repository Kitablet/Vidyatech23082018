
namespace FISE_Browser.Models
{
    public class UserBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool HasError { get; set; }
        public string SessionId { get; set; }
        public int UserId { get; set; }
    }
    public class User : UserBase
    {
        public string Email { get; set; }
        public bool Status { get; set; }
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
        public BooksDetail AllBooks { get; set; }
        public UserDetails UserDetails { get; set; }
    }

    public class LoginObject
    {
        public int Status { get; set; }
        public User User { get; set; }
    }

}
