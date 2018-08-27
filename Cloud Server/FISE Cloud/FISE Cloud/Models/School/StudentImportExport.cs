using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace FISE_Cloud.Models.School
{
    public class StudentImportExport
    {
        public StudentImportExport()
        {
            ErrorInfo = new Hashtable();
        }
        public int SNO { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string ParentEmail { get; set; }
        public string ParentMobileNumber { get; set; }
        public string Grade { get; set; }
        public string SubSection { get; set; }
        public string RollNo { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public Hashtable ErrorInfo { get; set; }
        public string Error { get; set; }
        public int RowNumber { get; set; }
        public bool Status { get; set; }
        public int TotalSuccess { get; set; }
        public int TotalFailed { get; set; }
        public bool GradeStatus { get; set; }
        public string IsRenew { get; set; }
        public string UniqueId { get; set; }
        public bool StudentStatus { get; set; }
        public bool IsAlreadyExists { get; set; }
    }
        
    public class errorobject
    {
        public string errormsg;
    }

    public class StudentImportExportInput
    {
        public StudentImportExportInput()
        {
            Students = new List<StudentImportExport>();
            ProcessedStudents = new List<StudentImportExport>();
        }
        public List<StudentImportExport> Students { get; set; }
        public List<StudentImportExport> ProcessedStudents { get; set; }
        public string SchoolUId { get; set; }
        public int FailedCount { get; set; }
        public ImportExportModalstatus ModalStatus { get; set; }
        public StudentsImportStatus Status { get; set; }

    }
    public enum StudentsImportStatus { Error = 0, Sucess = 1, InvalidSchool = 2 }
    public enum ImportExportModalstatus { Upload = 0, Error = 1, Import = 2, Summery = 3, InvalidExcel = 4,ProcessError }

    public class ChildImport
    {
        public ChildImport()
        {
            ErrorInfo = new Hashtable();
        }
        public int SNO { get; set; }
        public string ChildFirstName { get; set; }
        public string ChildLastName { get; set; }
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public string ParentEmailID { get; set; }
        public string Grade { get; set; }
        public string SubSection { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public Hashtable ErrorInfo { get; set; }
        public string Error { get; set; }
        public int RowNumber { get; set; }
        public bool Status { get; set; }
        public int TotalSuccess { get; set; }
        public int TotalFailed { get; set; }
        public bool GradeStatus { get; set; }
        public string IsRenew { get; set; }
        public string UniqueId { get; set; }
        public bool StudentStatus { get; set; }
        public bool IsAlreadyExists { get; set; }
    }
    public class ChildImportExportInput
    {
        public ChildImportExportInput()
        {
            Students = new List<ChildImport>();
            ProcessedStudents = new List<ChildImport>();
        }
        public List<ChildImport> Students { get; set; }
        public List<ChildImport> ProcessedStudents { get; set; }
        public string SchoolUId { get; set; }
        public int FailedCount { get; set; }
        public ImportExportModalstatus ModalStatus { get; set; }
        public StudentsImportStatus Status { get; set; }

    }
}