using DeviceInfo.Plugin;
using Kitablet.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace Kitablet
{
    public class AppData
    {
        private static readonly object padlock = new object();
        public static dynamic FileService { get; set; }
        public static User User { get; set; }
        public static DeviceDetail DeviceDetail { get; set; }
        public static UserDetails UserDetails { get; set; }
        public static UserProgressForSync UserProgress { get; set; }
        public static BooksDetail BooksDetail { get; set; }
        public static BooksStatus BooksStatus { get; set; }

        public static void InitailizeAppData()
        {
            if (FileService == null)
            {
                lock (padlock)
                {
                    if (FileService == null)
                    {
                        FileService = DependencyService.Get<ISaveAndLoad>();
                        InitailizeUser();
                        InitailizeBooksDetail();
                        InitailizeBooksStatus();
                    }
                }
            }
        }
        public static void InitailizeUser()
        {
            if (User == null)
            {
                User _User = null;
                try
                {
                    if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
                    {
                        Helpers.CacheSettings.SettingsKey = "UserDetails";
                        if (Helpers.CacheSettings.GeneralSettings != "")
                        {
                            _User = JsonConvert.DeserializeObject<User>(Helpers.CacheSettings.GeneralSettings);
                        }
                    }
                    else
                    {
                        if (FileService.getUserDetails("UserDetails") != null)
                        {
                            _User = FileService.getUserDetails("UserDetails");
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                User = _User == null ? new User() : _User;
            }
        }
        public static void InitailizeDeviceDetail()
        {
            if (DeviceDetail == null)
            {
                DeviceDetail device = new DeviceDetail();
                try
                {
                    string deviceName = FileService.getDeviceName();
                    if (string.IsNullOrEmpty(deviceName))
                    {
                        if (Device.OS == TargetPlatform.Android)
                        {
                            device.DeviceName = CrossDeviceInfo.Current.Model;
                        }
                        else
                        {
                            device.DeviceName = CrossDeviceInfo.Current.Model + "_" + Constant.UniqueID;
                        }
                    }
                    else
                    {
                        device.DeviceName = CrossDeviceInfo.Current.Model + "_" + deviceName;
                    }
                    switch (Constant.CurrentPlateform)
                    {
                        case "Android":
                            device.DeviceOS = CrossDeviceInfo.Current.Platform + " " + CrossDeviceInfo.Current.Version;
                            break;
                        case "iOS":
                            device.DeviceOS = "iOS";
                            break;
                        case "WinPhone":
                            device.DeviceOS = "WinPhone";
                            break;
                        case "UWP":
                            device.DeviceOS = "Windows 10";
                            break;
                        case "Win8.1":
                            device.DeviceOS = "Windows 8.1";
                            break;
                        default:
                            device.DeviceOS = "Unknown OS";
                            break;
                    }
                    device.DeviceDetails = Constant.UniqueID;
                    device.Platform = FileService.PlatformName();
                }
                catch (Exception ex)
                {

                }
                DeviceDetail = device;
            }
        }
        public static void InitailizeUserDetails()
        {
            if (UserDetails == null)
            {
                UserDetails _UserDetails = null;
                try
                {
                    if (FileService.CheckFileExistence("UserDetails.xml"))
                    {
                        XDocument UserDetailsXML = XDocument.Parse(FileService.LoadText("UserDetails.xml"));
                        if (UserDetailsXML != null && UserDetailsXML.Root != null)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(UserDetails));
                            _UserDetails = (UserDetails)serializer.Deserialize(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(UserDetailsXML.Root.ToString()))));
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                UserDetails = _UserDetails != null ? _UserDetails : new UserDetails
                {
                    UserId = User?.UserId.ToString(),
                    LastAccessedBookId = string.Empty,
                    LastReadLaterBookId = string.Empty,
                    TotalActivitiesCompleted = string.Empty,
                    TotalBookRated = string.Empty,
                    TotalBookRead = string.Empty,
                    TotalHourSpent = string.Empty,
                    TotalHourSpentOnActivity = string.Empty,
                    TotalHourSpentOnReading = string.Empty,
                    TotalHourSpentOnReview = string.Empty,
                    UserBooks = new UserBooks { UserBook = new List<UserBook>() }
                };
            }
            else
            {
                UserDetails = new UserDetails
                {
                    UserId = User?.UserId.ToString(),
                    LastAccessedBookId = string.Empty,
                    LastReadLaterBookId = string.Empty,
                    TotalActivitiesCompleted = string.Empty,
                    TotalBookRated = string.Empty,
                    TotalBookRead = string.Empty,
                    TotalHourSpent = string.Empty,
                    TotalHourSpentOnActivity = string.Empty,
                    TotalHourSpentOnReading = string.Empty,
                    TotalHourSpentOnReview = string.Empty,
                    UserBooks = new UserBooks { UserBook = new List<UserBook>() }
                };
            }
        }
        public static void InitailizeUserProgress()
        {
            if (UserProgress == null)
            {
                UserProgressForSync _UserProgress = null;
                try
                {
                    if (FileService.CheckFileExistence("UserSyncXml.xml"))
                    {
                        XDocument UserSyncXmlXML = XDocument.Parse(FileService.LoadText("UserSyncXml.xml"));
                        if (UserSyncXmlXML != null && UserSyncXmlXML.Root != null)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(UserProgressForSync));
                            _UserProgress = (UserProgressForSync)serializer.Deserialize(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(UserSyncXmlXML.Root.ToString()))));
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                UserProgress = _UserProgress != null ? _UserProgress : new UserProgressForSync
                {
                    BrowsingProgress = new BrowsingProgress
                    {
                        Progress = new List<Progress>()
                    },
                    DeviceId = Constant.DeviceIdFromDB.ToString(),
                    UserId = User != null ? User.UserId.ToString() : "0",
                    UserProgressBooks = new UserProgressBooks
                    {
                        UserProgressBook = new List<UserProgressBook>()
                    }
                };
            }
            else
            {
                UserProgress = new UserProgressForSync
                {
                    BrowsingProgress = new BrowsingProgress
                    {
                        Progress = new List<Progress>()
                    },
                    DeviceId = Constant.DeviceIdFromDB.ToString(),
                    UserId = User != null ? User.UserId.ToString() : "0",
                    UserProgressBooks = new UserProgressBooks
                    {
                        UserProgressBook = new List<UserProgressBook>()
                    }
                };
            }
        }
        public static void InitailizeBooksDetail()
        {
            if (BooksDetail == null)
            {
                BooksDetail _BooksDetail = null;
                try
                {
                    if (FileService.CheckFileExistence("Books.xml"))
                    {
                        XDocument BooksXML = XDocument.Parse(FileService.LoadText("Books.xml"));
                        if (BooksXML != null && BooksXML.Root != null)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(BooksDetail));
                            _BooksDetail = (BooksDetail)serializer.Deserialize(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(BooksXML.Root.ToString()))));
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                BooksDetail = _BooksDetail != null ? _BooksDetail : new BooksDetail
                {
                    Books = new Books
                    {
                        Book = new List<Book>()
                    },
                    BooksCount = string.Empty,
                    BookTypes = new BookTypes
                    {
                        BookType = new List<BookType>()
                    },
                    Genres = new Genres
                    {
                        Genre = new List<Genre>()
                    },
                    Languages = new Languages
                    {
                        Language = new List<Language>()
                    },
                    ReviewJson = string.Empty,
                    Status = string.Empty,
                    SubSections = new SubSections
                    {
                        SubSection = new List<SubSection>()
                    },
                    Tags = new Tags
                    {
                        Tag = new List<Tag>()
                    }
                };
            }
        }
        public static void InitailizeBooksStatus()
        {
            if (BooksStatus == null)
            {
                BooksStatus _BooksStatus = null;
                try
                {
                    if (FileService.CheckFileExistence("BooksStatus.xml"))
                    {
                        XDocument BooksStatusXML = XDocument.Parse(FileService.LoadText("BooksStatus.xml"));
                        if (BooksStatusXML != null && BooksStatusXML.Root != null)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(BooksStatus));
                            _BooksStatus = (BooksStatus)serializer.Deserialize(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(BooksStatusXML.Root.ToString()))));
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                BooksStatus = _BooksStatus != null ? _BooksStatus : new BooksStatus
                {
                    DownloadFile = new List<DownloadFile>()
                };
            }
        }
        public static void SaveAppData()
        {
            try
            {
                if (AppData.User != null)
                {
                    if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
                    {
                        Helpers.CacheSettings.SettingsKey = "UserDetails";
                        Helpers.CacheSettings.GeneralSettings = JsonConvert.SerializeObject(AppData.User);
                    }
                    else
                    {
                        FileService.saveUserDetails("UserDetails", AppData.User);
                    }
                }
                else
                {
                    if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
                    {
                        Helpers.CacheSettings.SettingsKey = "UserDetails";
                        Helpers.CacheSettings.GeneralSettings = "";
                    }
                    else
                    {
                        FileService.saveUserDetails("UserDetails", null);
                    }
                }
                if (AppData.UserDetails != null)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UserDetails));
                    XmlWriterSettings settings = new XmlWriterSettings();
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    using (StringWriter textWriter = new StringWriter())
                    {
                        using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                        {
                            serializer.Serialize(xmlWriter, AppData.UserDetails, ns);
                        }
                        FileService.SaveText("UserDetails.xml", textWriter.ToString());
                    }
                }
                else
                {
                    FileService.SaveText("UserDetails.xml", string.Empty);
                }
                if (AppData.UserProgress != null)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UserProgressForSync));
                    XmlWriterSettings settings = new XmlWriterSettings();
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    using (StringWriter textWriter = new StringWriter())
                    {
                        using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                        {
                            serializer.Serialize(xmlWriter, AppData.UserProgress, ns);
                        }
                        FileService.SaveText("UserSyncXml.xml", textWriter.ToString());
                    }
                }
                else
                {
                    FileService.SaveText("UserSyncXml.xml", string.Empty);
                }
                if (AppData.BooksDetail != null)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(BooksDetail));
                    XmlWriterSettings settings = new XmlWriterSettings();
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    using (StringWriter textWriter = new StringWriter())
                    {
                        using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                        {
                            serializer.Serialize(xmlWriter, AppData.BooksDetail, ns);
                        }
                        FileService.SaveText("Books.xml", textWriter.ToString());
                    }
                }
                if (AppData.BooksStatus != null)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(BooksStatus));
                    XmlWriterSettings settings = new XmlWriterSettings();
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    using (StringWriter textWriter = new StringWriter())
                    {
                        using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                        {
                            serializer.Serialize(xmlWriter, AppData.BooksStatus, ns);
                        }
                        FileService.SaveText("BooksStatus.xml", textWriter.ToString());
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
