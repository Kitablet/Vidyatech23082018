using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace FISE_Cloud.Models.School
{
    public class BaseBook
    {
        public int BookId { get; set; }
        public string FolderName { get; set; }
        public string KitabletID { get; set; }
        [AllowHtml]
        public string Title { get; set; }
        public string HinglishTitle { get; set; }
        [AllowHtml]
        public string ShortDescription { get; set; }
        [AllowHtml]
        public string Author { get; set; }
        [AllowHtml]
        public string Publisher { get; set; }
        [AllowHtml]
        public string Illustrator { get; set; }
        [AllowHtml]
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
    public class Book:BaseBook
    {        
        public int LanguageId { get; set; }
        public int SubSectionId { get; set; }
        public int BookTypeId { get; set; }
        public int GenreId { get; set; }

        public string Language { get; set; }
        public string SubSection { get; set; }
        public string Type { get; set; }
        public string Genre { get; set; }
        public int? Price { get; set; }
        public float Rating { get; set; }
    }

    public class BooksListAPIResult
    {
        public BooksListAPIResult()
        {
            Language = new List<Language>();
            BookType = new List<BookType>();
            SubSection = new List<SubSection>();
            Genre = new List<Genre>();
            Books = new APIPagedList<Book>();
        }
        public List<Language> Language { get; set; }
        public List<BookType> BookType { get; set; }
        public List<SubSection> SubSection { get; set; }
        public List<Genre> Genre { get; set; }
        public GenericStatus Status { get; set; }
        public APIPagedList<Book> Books { get; set; }
    }

    public class BooksListResult
    {
        public BooksListResult()
        {
            Language = new List<Language>();
            BookType = new List<BookType>();
            SubSection = new List<SubSection>();
            Genre = new List<Genre>();
            Book = new Book();
            SelectedLanguageIds = new List<int>();
            SelectedGenreIds = new List<int>();
            SelectedSubSectionIds = new List<int>();
            SelectedTypeIds = new List<int>();
        }
        public List<Language> Language { get; set; }
        public List<BookType> BookType { get; set; }
        public List<SubSection> SubSection { get; set; }
        public List<Genre> Genre { get; set; }
        public GenericStatus Status { get; set; }
        public PagedList<Book> Books { get; set; }
        public Book Book { get; set; }
        public List<int> SelectedLanguageIds { get; set; }
        public List<int> SelectedGenreIds { get; set; }
        public List<int> SelectedSubSectionIds { get; set; }
        public List<int> SelectedTypeIds { get; set; }
    }


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
    }


    public class Genre : FilterBase
    {
        //ADD Genre specific properties
    }
    public enum GenericStatus { Error = 0, Sucess = 1, Other = 3 }

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
}