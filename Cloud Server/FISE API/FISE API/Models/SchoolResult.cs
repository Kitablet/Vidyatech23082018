using System.Collections.Generic;

namespace FISE_API.Models
{
    public class SchoolResult
    {
        public SchoolStatus Status { get; set; }
        public School MySchool { get; set; }
    }
    public class SchoolData
    {
        public SchoolData()
        {
            this.Students = new PagedList<StudentModel>();
            this.Admins = new PagedList<UserCommon>();
            this.SchoolDetails = new School();
            Grades = new List<Grade>();
        }
        public School SchoolDetails { get; set; }
        public PagedList<StudentModel> Students { get; set; }
        public PagedList<UserCommon> Admins { get; set; }
        public List<Grade> Grades { get; set; }
        public int TotalStudents { get; set; }
        public SchoolStatus APIStatus { get; set; }
    }

    public class Grade
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}