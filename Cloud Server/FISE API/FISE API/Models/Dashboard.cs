using System.Collections.Generic;

namespace FISE_API.Models
{
    public class ParentDashboard : StudentModel
    {
        public int BooksRead { get; set; }
        public int BooksRated { get; set; }
        public int HourSpent { get; set; }
        public int ActivitiesCompleted { get; set; }
        public bool IsActive { get; set; }
        public bool SchoolIsTrashed { get; set; }
    }

    public class ParentDashboardResult
    {
        public ParentDashboardResult()
        {
            Students = new List<ParentDashboard>();
        }
        public List<ParentDashboard> Students { get; set; }
        public GenericStatus Status { get; set; }
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