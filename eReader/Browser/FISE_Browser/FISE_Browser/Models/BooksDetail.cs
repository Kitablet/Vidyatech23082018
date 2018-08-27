using System.Xml.Serialization;
using System.Collections.Generic;

namespace FISE_Browser.Models
{
    [XmlRoot(ElementName = "Language")]
    public class Language
    {
        [XmlElement(ElementName = "Id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "IsSelected")]
        public string IsSelected { get; set; }
        [XmlElement(ElementName = "BooksCount")]
        public string BooksCount { get; set; }
    }

    [XmlRoot(ElementName = "Languages")]
    public class Languages
    {
        [XmlElement(ElementName = "Language")]
        public List<Language> Language { get; set; }
    }

    [XmlRoot(ElementName = "BookType")]
    public class BookType
    {
        [XmlElement(ElementName = "Id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "IsSelected")]
        public string IsSelected { get; set; }
        [XmlElement(ElementName = "BooksCount")]
        public string BooksCount { get; set; }
    }

    [XmlRoot(ElementName = "BookTypes")]
    public class BookTypes
    {
        [XmlElement(ElementName = "BookType")]
        public List<BookType> BookType { get; set; }
    }

    [XmlRoot(ElementName = "SubSection")]
    public class SubSection
    {
        [XmlElement(ElementName = "Id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "IsSelected")]
        public string IsSelected { get; set; }
        [XmlElement(ElementName = "BooksCount")]
        public string BooksCount { get; set; }
        [XmlElement(ElementName = "ShortForm")]
        public string ShortForm { get; set; }
    }

    [XmlRoot(ElementName = "SubSections")]
    public class SubSections
    {
        [XmlElement(ElementName = "SubSection")]
        public List<SubSection> SubSection { get; set; }
    }

    [XmlRoot(ElementName = "Genre")]
    public class Genre
    {
        [XmlElement(ElementName = "Id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "IsSelected")]
        public string IsSelected { get; set; }
        [XmlElement(ElementName = "BooksCount")]
        public string BooksCount { get; set; }
    }

    [XmlRoot(ElementName = "Genres")]
    public class Genres
    {
        [XmlElement(ElementName = "Genre")]
        public List<Genre> Genre { get; set; }
    }

    [XmlRoot(ElementName = "TrashedOn")]
    public class TrashedOn
    {
        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Nil { get; set; }
        [XmlAttribute(AttributeName = "p4", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P4 { get; set; }
    }

    [XmlRoot(ElementName = "Search")]
    public class Search
    {
        [XmlElement(ElementName = "Language")]
        public string Language { get; set; }
        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "SubSection")]
        public string SubSection { get; set; }
        [XmlElement(ElementName = "Author")]
        public string Author { get; set; }
        [XmlElement(ElementName = "Illustrator")]
        public string Illustrator { get; set; }
        [XmlElement(ElementName = "Translator")]
        public string Translator { get; set; }
        [XmlElement(ElementName = "Genre")]
        public string Genre { get; set; }
        [XmlElement(ElementName = "Type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "HinglishTitle")]
        public string HinglishTitle { get; set; }
        [XmlElement(ElementName = "Publisher")]
        public string Publisher { get; set; }
        [XmlElement(ElementName = "ISBN")]
        public string ISBN { get; set; }
        [XmlElement(ElementName = "ShortDescription")]
        public string ShortDescription { get; set; }
    }

    [XmlRoot(ElementName = "Rating")]
    public class Rating
    {
        [XmlElement(ElementName = "AverageRating")]
        public string AverageRating { get; set; }
        [XmlElement(ElementName = "FiveStarRating")]
        public string FiveStarRating { get; set; }
        [XmlElement(ElementName = "FourStarRating")]
        public string FourStarRating { get; set; }
        [XmlElement(ElementName = "ThreeStarRating")]
        public string ThreeStarRating { get; set; }
        [XmlElement(ElementName = "TwoStarRating")]
        public string TwoStarRating { get; set; }
        [XmlElement(ElementName = "OneStarRating")]
        public string OneStarRating { get; set; }
        [XmlElement(ElementName = "TotalUserRatedThisBook")]
        public string TotalUserRatedThisBook { get; set; }
    }

    [XmlRoot(ElementName = "Book")]
    public class Book
    {
        [XmlElement(ElementName = "BookId")]
        public string BookId { get; set; }
        [XmlElement(ElementName = "FolderName")]
        public string FolderName { get; set; }
        [XmlElement(ElementName = "KitabletID")]
        public string KitabletID { get; set; }
        [XmlElement(ElementName = "YearOfPublication")]
        public string YearOfPublication { get; set; }
        [XmlElement(ElementName = "ViewMode")]
        public string ViewMode { get; set; }
        [XmlElement(ElementName = "PageDisplay")]
        public string PageDisplay { get; set; }
        [XmlElement(ElementName = "NoOfPages")]
        public string NoOfPages { get; set; }
        [XmlElement(ElementName = "HasAnimation")]
        public string HasAnimation { get; set; }
        [XmlElement(ElementName = "HasReadAloud")]
        public string HasReadAloud { get; set; }
        [XmlElement(ElementName = "HasActivity")]
        public string HasActivity { get; set; }
        [XmlElement(ElementName = "Thumbnail1")]
        public string Thumbnail1 { get; set; }
        [XmlElement(ElementName = "Thumbnail2")]
        public string Thumbnail2 { get; set; }
        [XmlElement(ElementName = "Thumbnail3")]
        public string Thumbnail3 { get; set; }
        [XmlElement(ElementName = "BackCover")]
        public string BackCover { get; set; }
        [XmlElement(ElementName = "IsTrashed")]
        public string IsTrashed { get; set; }
        [XmlElement(ElementName = "TrashedOn")]
        public TrashedOn TrashedOn { get; set; }
        [XmlElement(ElementName = "Search")]
        public Search Search { get; set; }
        [XmlElement(ElementName = "BookSize")]
        public string BookSize { get; set; }
        [XmlElement(ElementName = "Recommended")]
        public string Recommended { get; set; }
        [XmlElement(ElementName = "ComingSoon")]
        public string ComingSoon { get; set; }
        [XmlElement(ElementName = "TotalDownloads")]
        public string TotalDownloads { get; set; }
        [XmlElement(ElementName = "PopularityScore")]
        public string PopularityScore { get; set; }
        [XmlElement(ElementName = "TotalReadingTimeOfThisBook")]
        public string TotalReadingTimeOfThisBook { get; set; }
        [XmlElement(ElementName = "TotalUsersReadThisBook")]
        public string TotalUsersReadThisBook { get; set; }
        [XmlElement(ElementName = "TotalUsersCompletedReviewOfThisBook")]
        public string TotalUsersCompletedReviewOfThisBook { get; set; }
        [XmlElement(ElementName = "TotalUsersCompletedActivityOfThisBook")]
        public string TotalUsersCompletedActivityOfThisBook { get; set; }
        [XmlElement(ElementName = "TotalTimeForReviewOfThisBook")]
        public string TotalTimeForReviewOfThisBook { get; set; }
        [XmlElement(ElementName = "TotalTimeForActivityOfThisBook")]
        public string TotalTimeForActivityOfThisBook { get; set; }
        [XmlElement(ElementName = "Rating")]
        public Rating Rating { get; set; }
        [XmlElement(ElementName = "Languages")]
        public string Languages { get; set; }
        [XmlElement(ElementName = "SubSections")]
        public string SubSections { get; set; }
        [XmlElement(ElementName = "Types")]
        public string Types { get; set; }
        [XmlElement(ElementName = "Genres")]
        public string Genres { get; set; }
        [XmlElement(ElementName = "ActivityJson")]
        public string ActivityJson { get; set; }
        [XmlElement(ElementName = "isPagerAllowed")]
        public bool isPagerAllowed { get; set; }
    }

    [XmlRoot(ElementName = "Books")]
    public class Books
    {
        [XmlElement(ElementName = "Book")]
        public List<Book> Book { get; set; }
    }

    [XmlRoot(ElementName = "Tag")]
    public class Tag
    {
        [XmlElement(ElementName = "Text")]
        public string Text { get; set; }
        [XmlElement(ElementName = "Id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "Type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "Tags")]
    public class Tags
    {
        [XmlElement(ElementName = "Tag")]
        public List<Tag> Tag { get; set; }
    }

    [XmlRoot(ElementName = "UserBooksDetail")]
    public class BooksDetail
    {
        [XmlElement(ElementName = "Languages")]
        public Languages Languages { get; set; }
        [XmlElement(ElementName = "BookTypes")]
        public BookTypes BookTypes { get; set; }
        [XmlElement(ElementName = "SubSections")]
        public SubSections SubSections { get; set; }
        [XmlElement(ElementName = "Genres")]
        public Genres Genres { get; set; }
        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }
        [XmlElement(ElementName = "Books")]
        public Books Books { get; set; }
        [XmlElement(ElementName = "Tags")]
        public Tags Tags { get; set; }
        [XmlElement(ElementName = "BooksCount")]
        public string BooksCount { get; set; }
        [XmlElement(ElementName = "ReviewJson")]
        public string ReviewJson { get; set; }
    }

}
