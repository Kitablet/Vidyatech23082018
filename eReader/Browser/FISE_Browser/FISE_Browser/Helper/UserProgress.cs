using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace FISE_Browser.Helper
{
    [XmlRoot(ElementName = "Progress")]
    public class Progress
    {
        [XmlElement(ElementName = "StartTime")]
        public DateTime StartTime { get; set; }
        [XmlElement(ElementName = "EndTime")]
        public DateTime EndTime { get; set; }
    }

    [XmlRoot(ElementName = "BrowsingProgress")]
    public class BrowsingProgress
    {
        [XmlElement(ElementName = "Progress")]
        public List<Progress> Progress { get; set; }
    }
     


    [XmlRoot(ElementName = "ActivityProgress")]
    public class ActivityProgress : Progress
    {

    }
    [XmlRoot(ElementName = "ReviewProgress")]
    public class ReviewProgress : Progress
    {

    }

    [XmlRoot(ElementName = "Activity")]
    public class Activity
    {
        [XmlElement(ElementName = "IsActivityDone")]
        public bool IsActivityDone { get; set; }
        [XmlElement(ElementName = "ActivityCompletedOn")]
        public DateTime? ActivityCompletedOn { get; set; }
        [XmlElement(ElementName = "ActivityJson")]
        public string ActivityJson { get; set; }
        [XmlElement(ElementName = "ActivityProgress")]
        public List<ActivityProgress> ActivityProgress { get; set; }
    }

    public class BookReview
    {
        [XmlElement(ElementName = "IsReviewDone")]
        public bool IsReviewDone { get; set; }
        [XmlElement(ElementName = "ReviewCompletedOn")]
        public DateTime? ReviewCompletedOn { get; set; }
        [XmlElement(ElementName = "ReviewJson")]
        public string ReviewJson { get; set; }
        [XmlElement(ElementName = "ReviewProgress")]
        public List<ReviewProgress> ReviewProgress { get; set; }
    }

    [XmlRoot(ElementName = "Log")]
    public class Log
    {
        [XmlElement(ElementName = "Rating")]
        public int Rating { get; set; }
        [XmlElement(ElementName = "RatedOn")]
        public DateTime? RatedOn { get; set; }
    }

    [XmlRoot(ElementName = "RatingLog")]
    public class RatingLog
    {
        [XmlElement(ElementName = "Log")]
        public List<Log> Log { get; set; }
    }

    [XmlRoot(ElementName = "Page")]
    public class Page
    {
        [XmlElement(ElementName = "PageNumber")]
        public int PageNumber { get; set; }
        [XmlElement(ElementName = "StartTime")]
        public DateTime StartTime { get; set; }
        [XmlElement(ElementName = "EndTime")]
        public DateTime EndTime { get; set; }
    }

    [XmlRoot(ElementName = "Pages")]
    public class Pages
    {
        [XmlElement(ElementName = "Page")]
        public List<Page> Page { get; set; }
    }

    [XmlRoot(ElementName = "ReadingProgress")]
    public class ReadingProgress
    {
        [XmlElement(ElementName = "Pages")]
        public Pages Pages { get; set; }
    }

    [XmlRoot(ElementName = "UserProgressBook")]
    public class UserProgressBook
    {
        [XmlElement(ElementName = "BookId")]
        public int BookId { get; set; }
        [XmlElement(ElementName = "IsRead")]
        public bool IsRead { get; set; }
        [XmlElement(ElementName = "IsReadLater")]
        public bool IsReadLater { get; set; }
        [XmlElement(ElementName = "ReadLaterOn")]
        public DateTime? ReadLaterOn { get; set; }
        [XmlElement(ElementName = "LastDateAccessed")]
        public DateTime? LastDateAccessed { get; set; }
        [XmlElement(ElementName = "BookCompletedOn")]
        public DateTime? BookCompletedOn { get; set; }
        [XmlElement(ElementName = "CurrentPage")]
        public int CurrentPage { get; set; }
        [XmlElement(ElementName = "Activity")]
        public Activity Activity { get; set; }
        [XmlElement(ElementName = "RatingLog")]
        public RatingLog RatingLog { get; set; }
        [XmlElement(ElementName = "ReadingProgress")]
        public ReadingProgress ReadingProgress { get; set; }
        public BookReview BookReview { get; set; }
    }

    [XmlRoot(ElementName = "UserProgressBooks")]
    public class UserProgressBooks
    {
        [XmlElement(ElementName = "UserProgressBook")]
        public List<UserProgressBook> UserProgressBook { get; set; }
    }

    [XmlRoot(ElementName = "UserProgressDetails")]
    public class UserProgressForSync
    {
        [XmlElement(ElementName = "UserId")]
        public int UserId { get; set; }
        [XmlElement(ElementName = "DeviceId")]
        public int DeviceId { get; set; }
        [XmlElement(ElementName = "BrowsingProgress")]
        public BrowsingProgress BrowsingProgress { get; set; }
        [XmlElement(ElementName = "UserProgressBooks")]
        public UserProgressBooks UserProgressBooks { get; set; }
    }

    public class UserSyncDetails
    {
        public bool IsSynced { get; set; }
        public string UserDetails { get; set; }
    }
    public class UserSyncData
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Data { get; set; }
        public string SyncType { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsStatsSynced { get; set; }
    }
}