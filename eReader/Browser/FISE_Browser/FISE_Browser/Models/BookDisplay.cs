using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FISE_Browser.Models
{
    public class BookDisplay : Book
    {
        public int BookId { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string pagewidth1 { get; set; }
        public string pagewidth2 { get; set; }
        public List<OpfSpine> OpfSpine { get; set; }
        public int CurrentPage { get; set; }
        public int Status { get; set; }
        public bool IsCompleted { get; set; }
        public List<Page> page { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CompletedOn { get; set; }
    }

    public class OpfSpine
    {
        public string id { get; set; }
        public string href { get; set; }
        public string height1 { get; set; }
        public string height2 { get; set; }
        public string pageno { get; set; }
    }

    public class Page
    {
        public int PageNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
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
        public int Status { get; set; }
        public bool HasReadAloud { get; set; }
        public bool HasAnimation { get; set; }
        public int Rating { get; set; }
        public string ViewMode { get; set; }
        public string KitabletId { get; set; }
    }

    public class UAC
    {
        public int UserId { get; set; }
        public string TotalBookRead { get; set; }
        public string TotalBookRated { get; set; }
        public string TotalActivitiesCompleted { get; set; }
        public double TotalHourSpent { get; set; }
        public string LastAccessedBookId { get; set; }
        public string LastReadLaterBookId { get; set; }

        public string TotalHourSpentOnReading { get; set; }

        public string Thumbnail2 { get; set; }
        public bool HasActivity { get; set; }
        public bool HasAnimation { get; set; }
        public bool HasReadAloud { get; set; }
        public bool IsActivityDone { get; set; }
        public int SubSectionId { get; set; }
        public string ViewMode { get; set; }
        public int Rating { get; set; }
        public int BookId { get; set; }
    }
}