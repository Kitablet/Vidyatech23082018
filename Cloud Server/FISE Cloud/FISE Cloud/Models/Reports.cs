using System;
using System.Collections.Generic;
using FISE_Cloud.Models.School;
using Webdiyer.WebControls.Mvc;

namespace FISE_Cloud.Models
{
    public enum Report6Status { Error = 0, Filter = 1, Report = 2 }
    public class Report6
    {
        public List<SubSection> Subsections { get; set; }
        public List<Language> Languages { get; set; }
        public List<ReportSubsections> _ReportSubsections { get; set; }
        public Report6Status Status { get; set; }
    }

    public class ReportSubsections
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<ReportLanguages> _ReportLanguages { get; set; }
    }

    public class ReportLanguages
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public ReportLanguages()
        {
            Books = new List<Book>();
        }
        public List<Book> Books { get; set; }
    }

    public class RegistrationAndLogin
    {
        public PagedList<StudentRegistrationModel> Created { get; set; }
        public PagedList<StudentRegistrationModel> Registered { get; set; }
        public PagedList<StudentRegistrationModel> Active { get; set; }
    }

    public class RegistrationAndLoginResult
    {
        public APIPagedList<StudentRegistrationModel> Created { get; set; }
        public APIPagedList<StudentRegistrationModel> Registered { get; set; }
        public APIPagedList<StudentRegistrationModel> Active { get; set; }
    }

    public class Report7FilterModel
    {
        public List<string> Cities { get; set; }
        public List<FISE_Cloud.Models.School.School> Schools { get; set; }
    }

    public enum Quarter { First = 1, Second = 4, Third = 7, Fourth = 10 }
    public enum Month { January = 1, Feburary = 2, March = 3, April = 4, May = 5, June = 6, July = 7, August = 8, September = 9, October = 10, November = 11, December = 12 }

    public class UsageData
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public float Percent { get; set; }
    }

    public class UsageModel
    {
        public UsageModel()
        {
            Palteforms = new List<UsageData>();
            Environment = new List<UsageData>();
        }
        public List<UsageData> Palteforms { get; set; }
        public List<UsageData> Environment { get; set; }
    }

    public class Report7Model
    {
        public UsageModel UsageModel { get; set; }
        public List<string> Cities { get; set; }
        public List<FISE_Cloud.Models.School.School> Schools { get; set; }
    }

    public class Report7ExportModel
    {
        public string ActivityType { get; set; }
        public int ActivityDuration { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Platform { get; set; }
        public string Environment { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SchoolName { get; set; }
        public string Grade { get; set; }
    }

    public class Report2FilterModel : Report7FilterModel
    {
        public List<Grade> Grades { get; set; }
    }

    public class Report2Model
    {
        public List<Report2Cities> Cities { get; set; }
        public UsageModel UsageModel { get; set; }
        public List<FISE_Cloud.Models.School.School> Schools { get; set; }
    }

    public class Report2Cities
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Report2Schools> Schools { get; set; }
    }

    public class Report2Schools
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalStudents { get; set; }
        public int TotalHours { get; set; }
        public int AvgBooksRead { get; set; }
        public List<PieChart> Activity { set; get; }
        public List<Report7Grade> Grade { get; set; }
    }

    public class Report7Grade
    {
        public string Name { get; set; }
        public int AvgBooksRead { get; set; }
        public int TotalHours { get; set; }
        public List<PieChart> Activity { set; get; }
    }

    public class PieChart
    {
        public string Name { get; set; }
        public float Count { get; set; }
    }

    public class OverAllGrade
    {
        public int TotalStudents { get; set; }
        public List<PieChart> Activity { set; get; }
        public int Hourspent { get; set; }
    }

    public class Report3FilterModel
    {
        public List<string> Cities { get; set; }
        public List<SubSection> SubSections { get; set; }
        public List<Language> Languages { get; set; }
        public List<Book> Books { get; set; }
    }

    public class Report4Publisher
    {
        public string Publisher { get; set; }
        public string SubSection { get; set; }
        public string Language { get; set; }
    }

    public class Report4FilterModel
    {
        public Report4FilterModel()
        {
            Cities = new List<string>();
            SubSections = new List<SubSection>();
            Languages = new List<Language>();
            Publishers = new List<Report4Publisher>();
        }
        public List<string> Cities { get; set; }
        public List<SubSection> SubSections { get; set; }
        public List<Language> Languages { get; set; }
        public List<Report4Publisher> Publishers { get; set; }
    }

    public class Report3
    {
        public List<Report3Subsections> SubSection { get; set; }
    }

    public class Report3Subsections
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<Report3Languages> Language { get; set; }
    }

    public class Report3Languages
    {
        public string Name { get; set; }
        public List<ReportBook> ReportBook { get; set; }
    }

    public class ReportBook
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string ViewMode { get; set; }
        public string SubSection { get; set; }
        public float AvgReadingTime { get; set; }
        public float AvgRating { get; set; }
        public float AvgActivityTime { get; set; }
        public int TotalRating { get; set; }
        public int TotalActivity { get; set; }
        public int TotalRead { get; set; }
        public int ReadComplete { get; set; }
        public List<PieChart> Reading { get; set; }
        public List<PieChart> Rating { get; set; }
        public List<PieChart> Activity { get; set; }
    }

    public class Report4
    {
        public List<Report4Publishers> Publisher { get; set; }
    }

    public class Report4Publishers
    {
        public string Name { get; set; }
        public List<Report3Subsections> SubSection { get; set; }
    }

    public class Report4Subsections
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<Report3Languages> Language { get; set; }
    }

    public class Report4Languages
    {
        public string Name { get; set; }
        public List<ReportBook> ReportBook { get; set; }
    }

    public class Report8
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TotalBookRated { get; set; }
        public int TotalActivityCompleted { get; set; }
        public List<PieChart> Activities { get; set; }
        public List<Report8Book> Books { get; set; }
        public int TimeSpent { get; set; }
    }

    public class Report8Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public bool IsRated { get; set; }
        public bool IsActivityDone { get; set; }
        public int TimeSpent { get; set; }
    }

    public class Report2
    {
        public Report2()
        {
            Section = new List<string>();
        }
        public List<Report2City> City { get; set; }
        public List<Grade> Grade { get; set; }
        public List<string> Section { get; set; }
    }

    public class Report2City
    {
        public string Name { get; set; }
        public List<Report2School> School { get; set; }
    }

    public class Report2School
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public int OverAllTimeSpent { get; set; }
        public int OverAllTotalStudents { get; set; }
        public int OverAllAvgBookRead { get; set; }
        public List<PieChart> OverAllStudents { get; set; }
        public List<PieChart> OverAllActivitiesGrade { get; set; }
        public List<PieChart> OverAllActivitiesSubsection { get; set; }
        public List<Report2GradeSection> GradeWise { get; set; }
        public List<Report2GradeSection> SubSectionWise { get; set; }
        public Report2StudentWise StudentWise { get; set; }
        public List<Grade> Grade { get; set; }
        public List<string> Section { get; set; }
    }

    public class Report2GradeSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TimeSpent { get; set; }
        public int AvgBookRead { get; set; }
        public List<PieChart> Activities { get; set; }
    }

    public class Report2StudentWise
    {
        public List<Report8> StudentReport { get; set; }
        public List<StudentRegistrationModel> Students { get; set; }
        public List<string> Section { get; set; }
    }
    public class ChartModel
    {
        public string label { get; set; }
        public float value { get; set; }
        public string color { get; set; }
    }

    public class Report2Export
    {
        public string SchoolName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Section { get; set; }
        public float TimeSpentBookRead { get; set; }
        public float TimeSpentBrowsing { get; set; }
        public float TimeSpentActivity { get; set; }
        public int TotalBookRated { get; set; }
        public int TotalBookRead { get; set; }
        public int TotalActivitiesCompleted { get; set; }
        public string SubSection { get; set; }
        public string Grade { get; set; }
    }

    public class Dataset
    {
        public List<int> data { get; set; }
        public List<string> backgroundColor { get; set; }
    }

    public class RootObject
    {
        public RootObject()
        {
            labels = new List<string>();
            datasets = new List<Dataset>();
        }
        public List<string> labels { get; set; }
        public List<Dataset> datasets { get; set; }
    }

    public class Report5FilterModel
    {
        public List<string> Cities { get; set; }
        public List<Grade> Grades { get; set; }
        public List<FISE_Cloud.Models.School.School> Schools { get; set; }
    }

    public class Report5
    {
        public List<Report5Data> BookRead { get; set; }
        public List<Report5Data> TimeSpent { get; set; }

    }

    public class Report5Data
    {
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public float OverAll { get; set; }
        public float School { get; set; }
    }

    public class Report3Export
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string SubSection { get; set; }
        public string Publisher { get; set; }
        public float Rating { get; set; }
        public int Grade1Read { get; set; }
        public int Grade2Read { get; set; }
        public int Grade3Read { get; set; }
        public int Grade4Read { get; set; }
        public int Grade5Read { get; set; }
        public int Grade6Read { get; set; }
        public int Grade7Read { get; set; }
        public int Grade8Read { get; set; }
        public int Rating1 { get; set; }
        public int Rating2 { get; set; }
        public int Rating3 { get; set; }
        public int Rating4 { get; set; }
        public int Rating5 { get; set; }
        public int Grade1Activity { get; set; }
        public int Grade2Activity { get; set; }
        public int Grade3Activity { get; set; }
        public int Grade4Activity { get; set; }
        public int Grade5Activity { get; set; }
        public int Grade6Activity { get; set; }
        public int Grade7Activity { get; set; }
        public int Grade8Activity { get; set; }
        public int TotalRead { get; set; }
        public int TotalReadCompleted { get; set; }
    }

    public class Report5Export
    {
        public string OverAll { get; set; }
        public string OverAllGrade1 { get; set; }
        public string OverAllGrade2 { get; set; }
        public string OverAllGrade3 { get; set; }
        public string OverAllGrade4 { get; set; }
        public string OverAllGrade5 { get; set; }
        public string OverAllGrade6 { get; set; }
        public string OverAllGrade7 { get; set; }
        public string OverAllGrade8 { get; set; }

        public string School { get; set; }
        public string SchoolGrade1 { get; set; }
        public string SchoolGrade2 { get; set; }
        public string SchoolGrade3 { get; set; }
        public string SchoolGrade4 { get; set; }
        public string SchoolGrade5 { get; set; }
        public string SchoolGrade6 { get; set; }
        public string SchoolGrade7 { get; set; }
        public string SchoolGrade8 { get; set; }
    }

    public class Report8Export
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float ReadingTime { get; set; }
        public float ActivityTime { get; set; }
        public float BrowsingTime { get; set; }
        public string Rated { get; set; }
        public string ActivityDone { get; set; }
    }

    public class Report1SchoolAdmin {
        public List<Grade> Grade { get; set; }
        public List<string> Section { get; set; }
        public PagedList<StudentRegistrationModel> Created { get; set; }
        public PagedList<StudentRegistrationModel> Registered { get; set; }
        public PagedList<StudentRegistrationModel> Active { get; set; }
        public int CreatedTotal { get; set; }
        public int RegisteredTotal { get; set; }
        public int ActiveTotal { get; set; }
    }

    public class Report1SchoolAdminResult
    {
        public List<Grade> Grade { get; set; }
        public List<string> Section { get; set; }
        public APIPagedList<StudentRegistrationModel> Created { get; set; }
        public APIPagedList<StudentRegistrationModel> Registered { get; set; }
        public APIPagedList<StudentRegistrationModel> Active { get; set; }
        public int CreatedTotal { get; set; }
        public int RegisteredTotal { get; set; }
        public int ActiveTotal { get; set; }
    }
}