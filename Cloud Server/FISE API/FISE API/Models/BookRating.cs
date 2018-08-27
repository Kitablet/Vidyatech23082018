namespace FISE_API.Models
{
    public class BookRating
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
    }

    public class BookRatingResult
    {
        public BookRatingStatus Status { get; set; }
        public BookRating MyBookRating { get; set; }
    }
}