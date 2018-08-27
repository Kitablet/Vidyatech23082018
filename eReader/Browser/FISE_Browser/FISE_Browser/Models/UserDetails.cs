﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace FISE_Browser.Models
{
    [XmlRoot(ElementName = "ReviewCompletedOn")]
    public class ReviewCompletedOn
    {
        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Nil { get; set; }
        [XmlAttribute(AttributeName = "p4", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P4 { get; set; }
    }

    [XmlRoot(ElementName = "ActivityCompletedOn")]
    public class ActivityCompletedOn
    {
        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Nil { get; set; }
        [XmlAttribute(AttributeName = "p4", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P4 { get; set; }
    }

    [XmlRoot(ElementName = "RatedOn")]
    public class RatedOn
    {
        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Nil { get; set; }
        [XmlAttribute(AttributeName = "p4", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P4 { get; set; }
    }

    [XmlRoot(ElementName = "Devices")]
    public class Devices
    {
        [XmlElement(ElementName = "DeviceId")]
        public List<string> DeviceId { get; set; }
    }

    [XmlRoot(ElementName = "Bookmark")]
    public class Bookmark
    {
        [XmlElement(ElementName = "CurrentPage")]
        public string CurrentPage { get; set; }
        [XmlElement(ElementName = "ReadingTime")]
        public string ReadingTime { get; set; }
        [XmlElement(ElementName = "ReviewTime")]
        public string ReviewTime { get; set; }
        [XmlElement(ElementName = "ActivityTime")]
        public string ActivityTime { get; set; }
    }

    [XmlRoot(ElementName = "Book")]
    public class UserBook
    {
        [XmlElement(ElementName = "BookId")]
        public string BookId { get; set; }
        [XmlElement(ElementName = "Rating")]
        public string Rating { get; set; }
        [XmlElement(ElementName = "IsRead")]
        public string IsRead { get; set; }
        [XmlElement(ElementName = "IsActivityDone")]
        public string IsActivityDone { get; set; }
        [XmlElement(ElementName = "ActivityJson")]
        public string ActivityJson { get; set; }
        [XmlElement(ElementName = "IsReviewDone")]
        public string IsReviewDone { get; set; }
        [XmlElement(ElementName = "ReviewJson")]
        public string ReviewJson { get; set; }
        [XmlElement(ElementName = "IsReadLater")]
        public string IsReadLater { get; set; }
        [XmlElement(ElementName = "ReviewCompletedOn")]
        public string ReviewCompletedOn { get; set; }
        [XmlElement(ElementName = "ActivityCompletedOn")]
        public string ActivityCompletedOn { get; set; }
        [XmlElement(ElementName = "RatedOn")]
        public string RatedOn { get; set; }
        [XmlElement(ElementName = "Devices")]
        public Devices Devices { get; set; }
        [XmlElement(ElementName = "Bookmark")]
        public Bookmark Bookmark { get; set; }
        [XmlElement(ElementName = "BookCompletedOn")]
        public string BookCompletedOn { get; set; }
        [XmlElement(ElementName = "ReadLaterOn")]
        public string ReadLaterOn { get; set; }
        [XmlElement(ElementName = "LastDateAccessed")]
        public string LastDateAccessed { get; set; }
        [XmlElement(ElementName = "BookReadStartedOn")]
        public string BookReadStartedOn { get; set; }
    }

    [XmlRoot(ElementName = "BookCompletedOn")]
    public class BookCompletedOn
    {
        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Nil { get; set; }
        [XmlAttribute(AttributeName = "p4", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P4 { get; set; }
    }

    [XmlRoot(ElementName = "ReadLaterOn")]
    public class ReadLaterOn
    {
        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Nil { get; set; }
        [XmlAttribute(AttributeName = "p4", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P4 { get; set; }
    }

    [XmlRoot(ElementName = "LastDateAccessed")]
    public class LastDateAccessed
    {
        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Nil { get; set; }
        [XmlAttribute(AttributeName = "p4", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P4 { get; set; }
    }

    [XmlRoot(ElementName = "BookReadStartedOn")]
    public class BookReadStartedOn
    {
        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Nil { get; set; }
        [XmlAttribute(AttributeName = "p4", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P4 { get; set; }
    }

    [XmlRoot(ElementName = "Books")]
    public class UserBooks
    {
        [XmlElement(ElementName = "Book")]
        public List<UserBook> UserBook { get; set; }
    }

    [XmlRoot(ElementName = "UserDetails")]
    public class UserDetails
    {
        [XmlElement(ElementName = "UserId")]
        public string UserId { get; set; }
        [XmlElement(ElementName = "TotalBookRead")]
        public string TotalBookRead { get; set; }
        [XmlElement(ElementName = "TotalBookRated")]
        public string TotalBookRated { get; set; }
        [XmlElement(ElementName = "TotalActivitiesCompleted")]
        public string TotalActivitiesCompleted { get; set; }
        [XmlElement(ElementName = "TotalHourSpent")]
        public string TotalHourSpent { get; set; }
        [XmlElement(ElementName = "LastAccessedBookId")]
        public string LastAccessedBookId { get; set; }
        [XmlElement(ElementName = "LastReadLaterBookId")]
        public string LastReadLaterBookId { get; set; }
        [XmlElement(ElementName = "TotalHourSpentOnReading")]
        public string TotalHourSpentOnReading { get; set; }
        [XmlElement(ElementName = "TotalHourSpentOnReview")]
        public string TotalHourSpentOnReview { get; set; }
        [XmlElement(ElementName = "TotalHourSpentOnActivity")]
        public string TotalHourSpentOnActivity { get; set; }
        [XmlElement(ElementName = "Books")]
        public UserBooks UserBooks { get; set; }
        public BooksDetail AllBooks { get; set; }
    }
}