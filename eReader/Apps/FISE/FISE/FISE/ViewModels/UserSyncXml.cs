using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kitablet.ViewModels
{
    [XmlRoot(ElementName = "Progress")]
    public class Progress
    {
        [XmlElement(ElementName = "StartTime")]
        public string StartTime { get; set; }
        [XmlElement(ElementName = "EndTime")]
        public string EndTime { get; set; }
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
        public string IsActivityDone { get; set; }
        [XmlElement(ElementName = "ActivityCompletedOn")]
        public string ActivityCompletedOn { get; set; }
        [XmlElement(ElementName = "ActivityJson")]
        public string ActivityJson { get; set; }
        [XmlElement(ElementName = "ActivityProgress")]
        public List<ActivityProgress> ActivityProgress { get; set; }
    }

    [XmlRoot(ElementName = "BookReview")]
    public class BookReview
    {
        [XmlElement(ElementName = "IsReviewDone")]
        public string IsReviewDone { get; set; }
        [XmlElement(ElementName = "ReviewCompletedOn")]
        public string ReviewCompletedOn { get; set; }
        [XmlElement(ElementName = "ReviewJson")]
        public string ReviewJson { get; set; }
        [XmlElement(ElementName = "ReviewProgress")]
        public List<ReviewProgress> ReviewProgress { get; set; }
    }

    [XmlRoot(ElementName = "Log")]
    public class Log
    {
        [XmlElement(ElementName = "Rating")]
        public string Rating { get; set; }
        [XmlElement(ElementName = "RatedOn")]
        public string RatedOn { get; set; }
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
        public string PageNumber { get; set; }
        [XmlElement(ElementName = "StartTime")]
        public string StartTime { get; set; }
        [XmlElement(ElementName = "EndTime")]
        public string EndTime { get; set; }
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
        public string BookId { get; set; }
        [XmlElement(ElementName = "IsRead")]
        public string IsRead { get; set; }
        [XmlElement(ElementName = "IsReadLater")]
        public string IsReadLater { get; set; }
        [XmlElement(ElementName = "ReadLaterOn")]
        public string ReadLaterOn { get; set; }
        [XmlElement(ElementName = "LastDateAccessed")]
        public string LastDateAccessed { get; set; }
        [XmlElement(ElementName = "BookCompletedOn")]
        public string BookCompletedOn { get; set; }
        [XmlElement(ElementName = "CurrentPage")]
        public string CurrentPage { get; set; }
        [XmlElement(ElementName = "Activity")]
        public Activity Activity { get; set; }
        [XmlElement(ElementName = "RatingLog")]
        public RatingLog RatingLog { get; set; }
        [XmlElement(ElementName = "ReadingProgress")]
        public ReadingProgress ReadingProgress { get; set; }
        [XmlElement(ElementName = "BookReview")]
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
        public string UserId { get; set; }
        [XmlElement(ElementName = "DeviceId")]
        public string DeviceId { get; set; }
        [XmlElement(ElementName = "BrowsingProgress")]
        public BrowsingProgress BrowsingProgress { get; set; }
        [XmlElement(ElementName = "UserProgressBooks")]
        public UserProgressBooks UserProgressBooks { get; set; }
    }

    public class UserSyncDetails
    {
        public string IsSynced { get; set; }
        public string UserDetails { get; set; }
    }
}
