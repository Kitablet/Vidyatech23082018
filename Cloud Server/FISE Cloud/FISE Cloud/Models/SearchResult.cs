using FISE_Cloud.Models.School;
using Webdiyer.WebControls.Mvc;
namespace FISE_Cloud.Models
{
    public class Search
    {
        public APIPagedList<Book> Book { get; set; }
        public APIPagedList<FISE_Cloud.Models.School.School> School { get; set; }
        public APIPagedList<StudentRegistrationModel> Student { get; set; }
        public APIPagedList<UserRegistrationModel> SchoolAdmin { get; set; }
        public GenericStatus Status { get; set; }
    }

    public class SearchResult
    {
        public PagedList<Book> Book { get; set; }
        public PagedList<FISE_Cloud.Models.School.School> School { get; set; }
        public PagedList<StudentRegistrationModel> Student { get; set; }
        public PagedList<UserRegistrationModel> SchoolAdmin { get; set; }
        public GenericStatus Status { get; set; }
    }
}