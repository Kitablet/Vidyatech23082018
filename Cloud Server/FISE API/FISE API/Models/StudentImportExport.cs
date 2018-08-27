using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace FISE_API.Models
{
    public class StudentImportExport
    {
        public StudentImportExport()
        {
            ErrorInfo = new Hashtable();
        }
        public int SNO { get; set; }
        public int StudentId { get; set; }
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
        public bool Status { get; set; }
        public Hashtable ErrorInfo { get; set; }
        public string Error { get; set; }
        public int RowNumber { get; set; }
        public int TotalSuccess { get; set; }
        public bool GradeStatus { get; set; }
        public string IsRenew { get; set; }
        public string UniqueId { get; set; }
        public bool StudentStatus { get; set; }
        public bool IsAlreadyExists { get; set; }
        public static DataTable StudentImportExportTable(IList<StudentImportExport> _List)
        {
            DataTable dtImportexport = new DataTable();
            dtImportexport.Columns.Add("RowID", typeof(int));
            dtImportexport.Columns.Add("FirstName", typeof(string));
            dtImportexport.Columns.Add("LastName", typeof(string));
            dtImportexport.Columns.Add("Grade", typeof(string));
            dtImportexport.Columns.Add("Section", typeof(string));
            dtImportexport.Columns.Add("RollNo", typeof(string));
            dtImportexport.Columns.Add("Gender", typeof(string));
            dtImportexport.Columns.Add("DateOfBirth", typeof(DateTime));
            dtImportexport.Columns.Add("SubscriptionStartDate", typeof(string));
            dtImportexport.Columns.Add("SubscriptionEndDate", typeof(string));
            dtImportexport.Columns.Add("ParentEmail", typeof(string));
            dtImportexport.Columns.Add("ParentMobileNumber", typeof(string));
            dtImportexport.Columns.Add("UserID", typeof(int));
            int RowID = 0;
            foreach (StudentImportExport student in _List)
            {
                dtImportexport.Rows.Add(new object[]{++RowID,student.FirstName,student.LastName,student.Grade,student.SubSection,student.RollNo,
                                                 student.Gender,student.DateOfBirth,student.SubscriptionStartDate,student.SubscriptionEndDate,
                                                student.ParentEmail,student.ParentMobileNumber,student.StudentId});
            }

            return dtImportexport;
        }
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
        }
        public List<StudentImportExport> Students { get; set; }
        public string SchoolUId { get; set; }
        public StudentsImportStatus Status { get; set; }
    }    
    public class RegisterationEmailDetails
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
    }

    public class ChildImport
    {
        public ChildImport()
        {
            ErrorInfo = new Hashtable();
        }
        public int SNO { get; set; }
        public int StudentId { get; set; }
        public string ChildFirstName { get; set; }
        public string ChildLastName { get; set; }
        public string ParentEmailID { get; set; }
        public string Grade { get; set; }
        public string SubSection { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public bool Status { get; set; }
        public Hashtable ErrorInfo { get; set; }
        public string Error { get; set; }
        public int RowNumber { get; set; }
        public int TotalSuccess { get; set; }
        public bool GradeStatus { get; set; }
        public string IsRenew { get; set; }
        public string UniqueId { get; set; }
        public bool StudentStatus { get; set; }
        public bool IsAlreadyExists { get; set; }
        public static DataTable StudentImportExportTable(IList<ChildImport> _List)
        {
            DataTable dtImportexport = new DataTable();
            dtImportexport.Columns.Add("RowID", typeof(int));
            dtImportexport.Columns.Add("ChildFirstName", typeof(string));
            dtImportexport.Columns.Add("ChildLastName", typeof(string));
            dtImportexport.Columns.Add("Grade", typeof(string));
            dtImportexport.Columns.Add("SubscriptionStartDate", typeof(string));
            dtImportexport.Columns.Add("SubscriptionEndDate", typeof(string));
            dtImportexport.Columns.Add("ParentEmailID", typeof(string));
            dtImportexport.Columns.Add("UserID", typeof(int));
            int RowID = 0;
            foreach (ChildImport student in _List)
            {
                dtImportexport.Rows.Add(new object[]{++RowID,student.ChildFirstName,student.ChildLastName,student.Grade,student.SubscriptionStartDate,student.SubscriptionEndDate,
                                                student.ParentEmailID,student.StudentId});
            }

            return dtImportexport;
        }
    }
    public class ChildImportExportInput
    {
        public ChildImportExportInput()
        {
            Students = new List<ChildImport>();
        }
        public List<ChildImport> Students { get; set; }
        public string SchoolUId { get; set; }
        public StudentsImportStatus Status { get; set; }
    }
}