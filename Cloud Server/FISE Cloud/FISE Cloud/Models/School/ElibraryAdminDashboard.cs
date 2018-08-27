using System.Collections.Generic;

namespace FISE_Cloud.Models.School
{
    public class ElibraryAdminDashboard
    {
        public int TotalSchoolCount { get; set; }
        public int TotalStudentCount { get; set; }
        public int TotalBookCount { get; set; }
    }

    public class SuperAdminDashboard : ElibraryAdminDashboard
    {
        public int TotalElibraryAdminCount { get; set; }
    }

    public class ParentDashboard : StudentRegistrationModel
    {
        public int BooksRead { get; set; }
        public int BooksRated { get; set; }
        public int HourSpent { get; set; }
        public int ActivitiesCompleted { get; set; }
        public bool SchoolIsTrashed { get; set; }
        public bool IsActive { get; set; }
    }

    public class ParentDashboardModel
    {
        public List<ParentDashboard> Students { get; set; }
        public GenericStatus Status { get; set; }
    }
    public class ParentDashboardResult
    {
        public List<ParentDashboard> Active { get; set; }
        public List<ParentDashboard> ActivationPending { get; set; }
        public GenericStatus Status { get; set; }
    }

    public class Dashboards
    {
        public ElibraryAdminDashboard ElibraryAdmin_Dashboard { get; set; }
        public SuperAdminDashboard SuperAdmin_Dashboard { get; set; }
        public ParentDashboard Parent_Dashboard { get; set; }
    }

    public class ElibSuperAdminDashboardResult
    {
        public ElibSuperAdminDashboardResult()
        {
            SubSections = new List<SubSection>();
            Schools = new List<School>();
        }
        public int CitieCount { get; set; }
        public int SchoolCount { get; set; }
        public int StudentCount { get; set; }
        public int BookCount { get; set; }
        public int LibrarianCount { get; set; }
        public int HindiBookCount { get; set; }
        public int EnglishBookCount { get; set; }
        public int BilingualBookCount { get; set; }
        public List<SubSection> SubSections { get; set; }
        public List<School> Schools { get; set; }
        public GenericStatus Status { get; set; }
    }
}