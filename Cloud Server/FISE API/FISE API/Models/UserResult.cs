using System.Collections.Generic;

namespace FISE_API.Models
{
    public class UserResult
    {
        public UserStatus Status { get; set; }
        public User User { get; set; }
    }

    public class StudentResult
    {
        public UserStatus Status { get; set; }
        public StudentModel Student { get; set; }
    }

    public class StudentParentResult : StudentModel
    {
        public StudentParentResult()
        {
            ParentDetails = new User();
            Students = new List<StudentModel>();
        }
        public User ParentDetails { get; set; }
        public List<StudentModel> Students { get; set; }
        public UserStatus APIStatus { get; set; }
        
    }

    public class StudentProfileResult : StudentModel
    {
        public string Grades { get; set; }
        public UserStatus APIStatus { get; set; }
    }
}