namespace FISE_API.Models
{
    public class SearchResult
    {
        public SearchResult()
        {
            Book = new PagedList<Book>();
            SchoolAdmin = new PagedList<StudentModel>();
            School = new PagedList<School>();
            Student = new PagedList<StudentModel>();
        }
        public PagedList<Book> Book { get; set; }
        public PagedList<School> School { get; set; }
        public PagedList<StudentModel> Student { get; set; }
        public PagedList<StudentModel> SchoolAdmin { get; set; }
        public GenericStatus Status { get; set; }
    }
}