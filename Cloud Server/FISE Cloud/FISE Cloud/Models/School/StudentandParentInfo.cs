using System.Collections.Generic;

namespace FISE_Cloud.Models.School
{
    public class StudentandParentInfo : StudentRegistrationModel
    {
        public UserRegistrationModel ParentDetails { get; set; }
        public List<StudentRegistrationModel> Students { get; set; }
        public int APIStatus { get; set; }
    }
}
