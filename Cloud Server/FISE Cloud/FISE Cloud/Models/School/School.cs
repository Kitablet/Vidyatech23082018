using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Webdiyer.WebControls.Mvc;

namespace FISE_Cloud.Models.School
{
    public class School : AddressFields
    {
        public int SchoolId { get; set; }
        public string SchoolUId { get; set; }
        [Display(Name = "School Name")]
        public string SchoolName { get; set; }
        [Display(Name = "Principal Name")]
        public string PrincipalName { get; set; }
        [Display(Name = "Principal Email")]
        public string PrincipalEmail { get; set; }
        public bool IsTrashed { get; set; }
        public DateTime? TrashedOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public string Result { get; set; }
        public int StudentCount { get; set; }
        public int SchoolAdminCount { get; set; }
        public bool IsEmailVerified { get; set; }
    }
    public class SchoolDetails
    {
        public School SchoolDetail { get; set; }
        public PagedList<StudentRegistrationModel> Students { get; set; }
        public PagedList<UserRegistrationModel> Admins { get; set; }
        public List<Grade> Grades { get; set; }
        public int TotalStudents { get; set; }

    }
    public class SchoolDetailsResult
    {
        public School SchoolDetails { get; set; }
        public APIPagedList<StudentRegistrationModel> Students { get; set; }
        public APIPagedList<UserRegistrationModel> Admins { get; set; }
        public List<Grade> Grades { get; set; }
        public int TotalStudents { get; set; }
        public SchoolStatusEnum APIStatus { get; set; }
    }
    public class Grade
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}