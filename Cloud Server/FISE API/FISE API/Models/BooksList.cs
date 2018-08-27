using System;
using System.Collections.Generic;
using System.Xml.Serialization;
namespace FISE_API.Models
{

    public class FilterBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public int BooksCount { get; set; }
    }

    public class Language : FilterBase
    {
        //ADD Language specific properties
    }

    public class BookType : FilterBase
    {
        //ADD BookType specific properties
    }


    public class SubSection : FilterBase
    {
        //ADD Subsection specific properties
        public string ShortForm { get; set; }
    }


    public class Genre : FilterBase
    {
        //ADD Genre specific properties
    }

    public class BooksListResult
    {
        public BooksListResult()
        {
            Language = new List<Language>();
            BookType = new List<BookType>();
            SubSection = new List<SubSection>();
            Books = new PagedList<Book>();
        }
        public List<Language> Language { get; set; }
        public List<BookType> BookType { get; set; }
        public List<SubSection> SubSection { get; set; }
        public GenericStatus Status { get; set; }
        public PagedList<Book> Books { get; set; }
    }
    public class BaseBook
    {
        public int BookId { get; set; }
        public string FolderName { get; set; }
        public string KitabletID { get; set; }
        public string Title { get; set; }
        public string HinglishTitle { get; set; }
        public string ShortDescription { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Illustrator { get; set; }
        public string Translator { get; set; }
        public string ISBN { get; set; }
        public int YearOfPublication { get; set; }
        public string ViewMode { get; set; }
        public string PageDisplay { get; set; }
        public int NoOfPages { get; set; }
        public bool HasAnimation { get; set; }
        public bool HasReadAloud { get; set; }
        public bool HasActivity { get; set; }
        public string Thumbnail1 { get; set; }
        public string Thumbnail2 { get; set; }
        public string Thumbnail3 { get; set; }
        public string BackCover { get; set; }
        public bool IsTrashed { get; set; }
        public DateTime? TrashedOn { get; set; }
    }
    public class Book : BaseBook
    {
        public string Language { get; set; }
        public string SubSection { get; set; }
        public string Type { get; set; }
        public string Genre { get; set; }
        public int? Price { get; set; }
        public float Rating { get; set; }
    }

    public class BooksDetailsResult
    {
        public BooksDetailsResult()
        {
            Language = new List<Language>();
            BookType = new List<BookType>();
            SubSection = new List<SubSection>();
            Genre = new List<Genre>();
            Book = new Book();
        }
        public List<Language> Language { get; set; }
        public List<BookType> BookType { get; set; }
        public List<SubSection> SubSection { get; set; }
        public List<Genre> Genre { get; set; }
        public GenericStatus Status { get; set; }
        public Book Book { get; set; }
    }

    #region UserBooksDetail

    [XmlRoot("UserBooksDetail")]
    public class UserBooksDetail
    {
        public UserBooksDetail()
        {
            Languages = new List<Language>();
            BookTypes = new List<BookType>();
            SubSections = new List<SubSection>();
            Genres = new List<Genre>();
            Books = new List<UserBook>();
            Tags = new List<Tag>();
        }
        [XmlArray("Languages")]
        [XmlArrayItem("Language")]
        public List<Language> Languages { get; set; }
        [XmlArray("BookTypes")]
        [XmlArrayItem("BookType")]
        public List<BookType> BookTypes { get; set; }
        [XmlArray("SubSections")]
        [XmlArrayItem("SubSection")]
        public List<SubSection> SubSections { get; set; }
        [XmlArray("Genres")]
        [XmlArrayItem("Genre")]
        public List<Genre> Genres { get; set; }
        public GenericStatus Status { get; set; }
        [XmlArray("Books")]
        [XmlArrayItem("Book")]
        public List<UserBook> Books { get; set; }
        public List<Tag> Tags { get; set; }
        public int BooksCount { get; set; }
        public string ReviewJson { get; set; }
    }

    public class Tag
    {
        public string Text { get; set; }
        public int Id { get; set; }
        public string Type { get; set; }
    }
    public class SerachBook
    {
        public string Language { get; set; }
        public string Title { get; set; }
        public string SubSection { get; set; }
        public string Author { get; set; }
        public string Illustrator { get; set; }
        public string Translator { get; set; }
        public string Genre { get; set; }
        public string Type { get; set; }
        public string HinglishTitle { get; set; }
        public string Publisher { get; set; }
        public string ISBN { get; set; }
        public string ShortDescription { get; set; }

    }
    public class Rating
    {
        public double AverageRating { get; set; }
        public int FiveStarRating { get; set; }
        public int FourStarRating { get; set; }
        public int ThreeStarRating { get; set; }
        public int TwoStarRating { get; set; }
        public int OneStarRating { get; set; }
        public int TotalUserRatedThisBook { get; set; }

    }

    public class ExportBook : Book
    {
        public ExportBook()
        {
            Languages = new List<string>();
            SubSections = new List<string>();
            Types = new List<string>();
            Genres = new List<string>();
        }
        public List<string> Languages { get; set; }
        public List<string> SubSections { get; set; }
        public List<string> Types { get; set; }
        public List<string> Genres { get; set; }
        public string Enabled { get; set; }
    }


    public class UserBook : BaseBook
    {
        public UserBook()
        {
            Search = new SerachBook();
            Rating = new Rating();
        }
        public SerachBook Search { get; set; }
        public double BookSize { get; set; }
        public bool Recommended { get; set; }
        public bool ComingSoon { get; set; }
        public int TotalDownloads { get; set; }
        public int PopularityScore { get; set; }
        public int TotalReadingTimeOfThisBook { get; set; }
        public int TotalUsersReadThisBook { get; set; }
        public int TotalUsersCompletedReviewOfThisBook { get; set; }
        public int TotalUsersCompletedActivityOfThisBook { get; set; }
        public int TotalTimeForReviewOfThisBook { get; set; }
        public int TotalTimeForActivityOfThisBook { get; set; }
        public Rating Rating { get; set; }
        public string Languages { get; set; }
        public string SubSections { get; set; }
        public string Types { get; set; }
        public string Genres { get; set; }
        public string ActivityJson { get; set; }
        public bool IsPagerAllowed { get; set; }
    }
    public class UserDetails
    {
        public UserDetails()
        {
            Books = new List<DeviceBook>();
        }
        public int UserId { get; set; }
        public int TotalBookRead { get; set; }
        public int TotalBookRated { get; set; }
        public int TotalActivitiesCompleted { get; set; }
        public double TotalHourSpent { get; set; }
        public int LastAccessedBookId { get; set; }
        public int LastReadLaterBookId { get; set; }

        public int TotalHourSpentOnReading { get; set; }
        public int TotalHourSpentOnReview { get; set; }
        public int TotalHourSpentOnActivity { get; set; }

        [XmlArray("Books")]
        [XmlArrayItem("Book")]
        public List<DeviceBook> Books { get; set; }
    }

    public class DeviceBook
    {
        public DeviceBook()
        {
            DeviceId = new List<int>();
            Bookmark = new Bookmark();
        }
        public int BookId { get; set; }
        public int Rating { get; set; }
        public bool IsRead { get; set; }
        public bool IsActivityDone { get; set; }
        public string ActivityJson { get; set; }
        public bool IsReviewDone { get; set; }
        public string ReviewJson { get; set; }
        public bool IsReadLater { get; set; }
        public DateTime? LastDateAccessed { get; set; }
        public DateTime? ReviewCompletedOn { get; set; }
        public DateTime? ActivityCompletedOn { get; set; }
        public DateTime? BookCompletedOn { get; set; }

        public DateTime? BookReadStartedOn { get; set; }
        public DateTime? RatedOn { get; set; }
        public DateTime? ReadLaterOn { get; set; }

        [XmlArray("Devices")]
        [XmlArrayItem("DeviceId")]
        public List<int> DeviceId { get; set; }
        public Bookmark Bookmark { get; set; }
    }


    public class Device
    {
        public int DeviceId { get; set; }
        public string DeviceDetails { get; set; }
        public string DeviceName { get; set; }
        public string DeviceOS { get; set; }
        public string Environment { get; set; }
    }
    public class Bookmark
    {
        public int CurrentPage { get; set; }
        public int ReadingTime { get; set; }
        public int ReviewTime { get; set; }
        public int ActivityTime { get; set; }
    }

    public class BookRead : BaseBook
    {
        public BookRead()
        {
            Rating = new Rating();
        }
        public Rating Rating { get; set; }
        public bool IsActivityDone { get; set; }
        public bool IsReadLater { get; set; }
        public bool IsRead { get; set; }
        public int UserRating { get; set; }
        public int SubSection { get; set; }
        public BookStatus Status { get; set; }
        public bool IsReviewDone { get; set; }
        public string SubSectionName { get; set; }
        public string ReviewJson { get; set; }
        public int CurrentPage { get; set; }
    }


    public class BookDisplay : BaseBook
    {
        public int CurrentPage { get; set; }
        public GenericStatus Status { get; set; }
        public bool IsPagerAllowed { get; set; }
        public bool IsCompleted { get; set; }
        public List<Page> page { get; set; }
        public int UserId { get; set; }
    }

    public class BookActivity
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public bool IsActivityDone { get; set; }
        public string Json { get; set; }
        public DateTime CompletedOn { get; set; }
        public int CompletionTime { get; set; }
        public string Environment { get; set; }
        public string Platform { get; set; }
        public BookStatus Status { get; set; }
        public bool HasReadAloud { get; set; }
        public bool HasAnimation { get; set; }
        public int Rating { get; set; }
        public string ViewMode { get; set; }
        public string KitabletId { get; set; }
    }

    public class UAC
    {
        public int UserId { get; set; }
        public int TotalBookRead { get; set; }
        public int TotalBookRated { get; set; }
        public int TotalActivitiesCompleted { get; set; }
        public double TotalHourSpent { get; set; }
        public int LastAccessedBookId { get; set; }
        public int LastReadLaterBookId { get; set; }

        public int TotalHourSpentOnReading { get; set; }
        public int TotalHourSpentOnReview { get; set; }
        public int TotalHourSpentOnActivity { get; set; }

        public string Thumbnail2 { get; set; }
        public bool HasActivity { get; set; }
        public bool HasAnimation { get; set; }
        public bool HasReadAloud { get; set; }
        public bool IsActivityDone { get; set; }
        public int Rating { get; set; }
        public int BookId { get; set; }
        public string ViewMode { get; set; }
        public int SubSectionId { get; set; }
    }
    #endregion
}
