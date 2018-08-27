namespace FISE_Browser.Models
{
    public class BookRead :Search
    {
        public bool IsActivityDone { get; set; }
        public bool IsReadLater { get; set; }
        public bool IsRead { get; set; }
        public int UserRating { get; set; }
        
        public int Status { get; set; }
        public int BookId { get; set; }
        public Rating Rating { get; set; }
        public bool IsReviewDone { get; set; }
        public string ViewMode { get; set; }
        public string Thumbnail3 { get; set; }
        public bool HasReadAloud {get;set; }
        public bool HasActivity { get; set; }
        public bool HasAnimation { get; set; }
        public string SubSectionName { get; set; }
        public string ReviewJson { get; set; }
        public int CurrentPage { get; set; }
        public string KitabletID { get; set; }
    }

    public class OpenBook : Book
    {
        public int CurrentPage { get; set; }
        public int Status { get; set; }
        public bool IsPagerAllowed { get; set; }
    }
}