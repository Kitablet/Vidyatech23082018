using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Kitablet.ViewModels
{
    public class Utils
    {
        public enum UpdateMethod { Browsing, Books, Total, New };

        public static void SetBrowsingProgress(DateTime StartTime)
        {
            try
            {
                if (AppData.UserProgress != null)
                {
                    if (AppData.UserProgress.BrowsingProgress.Progress == null)
                    {
                        AppData.UserProgress.BrowsingProgress.Progress = new List<Progress>();
                    }
                    AppData.UserProgress.BrowsingProgress.Progress.Add(new Progress
                    {
                        StartTime = StartTime.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                        EndTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture)
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void SetBookReadingTime(string BookId, DateTime StartTime, DateTime EndTime)
        {
            try
            {
                TimeSpan difference = EndTime.Subtract(StartTime);
                if (AppData.UserDetails != null)
                {
                    UserBook _book = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    if (_book != null)
                    {
                        if (!string.IsNullOrEmpty(_book.Bookmark.ReadingTime?.Trim()))
                        {
                            string[] readingTime = _book.Bookmark.ReadingTime.Trim().Split(':');
                            if (readingTime.Length == 1)
                            {
                                int minutes = Int32.Parse(readingTime[0]);
                                difference = difference.Add(new TimeSpan((int)minutes / 60, (int)minutes % 60, 0));
                            }
                            if (readingTime.Length == 3)
                            {
                                difference = difference.Add(new TimeSpan(Convert.ToInt32(readingTime[0]), Convert.ToInt32(readingTime[1]), Convert.ToInt32(readingTime[2])));
                            }
                            _book.Bookmark.ReadingTime = Convert.ToInt32(difference.TotalMinutes).ToString();
                        }
                        else
                        {
                            _book.Bookmark.ReadingTime = Convert.ToInt32(difference.TotalMinutes).ToString();
                        }
                    }
                }

                UpdateUserBookStatus();
                SetBookAccess(BookId);
            }
            catch (Exception ex)
            {

            }
        }

        public static void SetBookReviewTime(string BookId, DateTime StartTime, DateTime EndTime)
        {
            try
            {
                TimeSpan difference = EndTime.Subtract(StartTime);

                Utils.AddUserSyncBook(BookId);

                if (AppData.UserDetails != null && AppData.UserProgress != null)
                {
                    UserBook _book = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    if (_book != null)
                    {
                        if (!string.IsNullOrEmpty(_book.Bookmark.ReviewTime?.Trim()))
                        {
                            string[] reviewTime = _book.Bookmark.ReviewTime.Trim().Split(':');
                            if (reviewTime.Length == 1)
                            {
                                int minutes = Int32.Parse(reviewTime[0]);
                                difference = difference.Add(new TimeSpan((int)minutes / 60, (int)minutes % 60, 0));
                            }
                            if (reviewTime.Length == 3)
                            {
                                difference = difference.Add(new TimeSpan(Convert.ToInt32(reviewTime[0]), Convert.ToInt32(reviewTime[1]), Convert.ToInt32(reviewTime[2])));
                            }
                            _book.Bookmark.ReviewTime = Convert.ToInt32(difference.TotalMinutes).ToString();
                        }
                        else
                        {
                            _book.Bookmark.ReviewTime = Convert.ToInt32(difference.TotalMinutes).ToString();
                        }
                        AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault()
                            .BookReview.ReviewProgress.Add(new ReviewProgress
                            {
                                StartTime = StartTime.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                EndTime = EndTime.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture)
                            });
                    }
                }

                UpdateUserBookStatus();
                SetBookAccess(BookId);
            }
            catch (Exception ex)
            {

            }
        }

        public static void SetBookActivityTime(string BookId, DateTime StartTime, DateTime EndTime)
        {
            try
            {
                TimeSpan difference = EndTime.Subtract(StartTime);

                if (AppData.UserDetails != null)
                {
                    UserBook _book = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    if (_book != null)
                    {
                        if (!string.IsNullOrEmpty(_book.Bookmark.ActivityTime?.Trim()))
                        {
                            string[] activityTime = _book.Bookmark.ActivityTime.Trim().Split(':');
                            if (activityTime.Length == 1)
                            {
                                int minutes = Int32.Parse(activityTime[0]);
                                difference = difference.Add(new TimeSpan((int)minutes / 60, (int)minutes % 60, 0));
                            }
                            if (activityTime.Length == 3)
                            {
                                difference = difference.Add(new TimeSpan(Convert.ToInt32(activityTime[0]), Convert.ToInt32(activityTime[1]), Convert.ToInt32(activityTime[2])));
                            }
                            _book.Bookmark.ActivityTime = Convert.ToInt32(difference.TotalMinutes).ToString();
                        }
                        else
                        {
                            _book.Bookmark.ActivityTime = Convert.ToInt32(difference.TotalMinutes).ToString();
                        }
                    }
                }

                Utils.AddUserSyncBook(BookId);

                if (AppData.UserProgress != null)
                {
                    UserProgressBook _userBookProgress = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    if (_userBookProgress != null)
                    {
                        if (_userBookProgress.Activity.ActivityProgress == null)
                        {
                            _userBookProgress.Activity.ActivityProgress = new List<ActivityProgress>();
                        }
                        _userBookProgress.Activity.ActivityProgress.Add(new ActivityProgress
                        {
                            StartTime = StartTime.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                            EndTime = EndTime.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture)
                        });
                    }
                }

                UpdateUserBookStatus();
                SetBookAccess(BookId);
            }
            catch (Exception ex)
            {

            }
        }

        public static void SetPageProgress(string BookId, List<int> PageIndex, DateTime StartTime, DateTime EndTime)
        {
            try
            {
                Utils.AddUserSyncBook(BookId);

                if (AppData.UserProgress != null)
                {
                    Pages Pages = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault().ReadingProgress.Pages;
                    if (Pages != null)
                    {
                        if (Pages.Page == null)
                        {
                            Pages.Page = new List<Page>();
                        }
                        foreach (int pageNo in PageIndex)
                        {
                            if (pageNo != 0)
                            {
                                Pages.Page.Add(new Page
                                {
                                    PageNumber = pageNo.ToString(),
                                    StartTime = StartTime.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                    EndTime = EndTime.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void SetRatingLog(string BookId, int Rate)
        {
            try
            {
                Utils.AddUserSyncBook(BookId);

                if (AppData.UserProgress != null)
                {
                    RatingLog RatingLog = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault().RatingLog;
                    if (RatingLog != null)
                    {
                        if (RatingLog.Log == null)
                        {
                            RatingLog.Log = new List<Log>();
                        }
                        if (Rate != 0)
                        {
                            RatingLog.Log.Add(new Log
                            {
                                Rating = Rate.ToString(),
                                RatedOn = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void SetBookCurrentPage(string BookId, int currentPage)
        {
            try
            {
                Utils.AddUserSyncBook(BookId);

                if (AppData.UserDetails != null && AppData.UserProgress != null)
                {
                    UserBook _book = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    UserProgressBook _bookProgress = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    if (_book != null && _bookProgress != null)
                    {
                        _book.Bookmark.CurrentPage = currentPage.ToString();
                        _bookProgress.CurrentPage = currentPage.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void SetBookAccess(string BookId)
        {
            try
            {
                Utils.AddUserSyncBook(BookId);

                if (AppData.UserDetails != null && AppData.UserProgress != null)
                {
                    UserBook _book = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    UserProgressBook _bookProgress = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    if (_book != null && _bookProgress != null)
                    {
                        _book.LastDateAccessed = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
                        _bookProgress.LastDateAccessed = _book.LastDateAccessed;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void SetBookCompleted(string BookId)
        {
            try
            {
                Utils.AddUserSyncBook(BookId);

                if (AppData.UserDetails != null && AppData.UserProgress != null)
                {
                    UserBook _book = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    UserProgressBook _bookProgress = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    if (_book != null && _bookProgress != null)
                    {
                        _book.IsRead = "true";
                        _book.Bookmark.CurrentPage = "0";
                        _bookProgress.IsRead = "true";
                        _bookProgress.CurrentPage = "0";
                        if (string.IsNullOrEmpty(_book.BookCompletedOn?.Trim()))
                        {
                            _book.BookCompletedOn = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
                        }
                        _bookProgress.BookCompletedOn = _book.BookCompletedOn;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void SetBookReviewCompleted(string BookId, UserBookReview bookReview)
        {
            try
            {
                Utils.AddUserSyncBook(BookId);

                if (AppData.UserDetails != null && AppData.UserProgress != null)
                {
                    UserBook _book = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    UserProgressBook _bookProgress = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    if (_book != null && _bookProgress != null)
                    {
                        _book.IsReviewDone = "true";
                        _book.ReviewJson = JsonConvert.SerializeObject(bookReview);
                        _bookProgress.BookReview.IsReviewDone = "true";
                        _bookProgress.BookReview.ReviewJson = _book.ReviewJson;
                        if (string.IsNullOrEmpty(_book.ReviewCompletedOn?.Trim()))
                        {
                            _book.ReviewCompletedOn = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
                        }
                        _bookProgress.BookReview.ReviewCompletedOn = _book.ReviewCompletedOn;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void SetBookActivityCompleted(string BookId, string ActivityJson)
        {
            try
            {
                Utils.AddUserSyncBook(BookId);

                if (AppData.UserDetails != null && AppData.UserProgress != null)
                {
                    UserBook _book = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    UserProgressBook _bookProgress = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    if (_book != null && _bookProgress != null)
                    {
                        _book.IsActivityDone = "true";
                        _book.ActivityJson = ActivityJson;
                        _bookProgress.Activity.IsActivityDone = "true";
                        _bookProgress.Activity.ActivityJson = ActivityJson;
                        if (string.IsNullOrEmpty(_book.ActivityCompletedOn?.Trim()))
                        {
                            _book.ActivityCompletedOn = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
                        }
                        _bookProgress.Activity.ActivityCompletedOn = _book.ActivityCompletedOn;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static UserBook AddNewBookElement(string BookId, string IsReadLater, List<string> DeviceId)
        {
            try
            {
                UserBook _book = new UserBook
                {
                    BookId = BookId,
                    Rating = string.Empty,
                    IsRead = "false",
                    ActivityJson = string.Empty,
                    IsActivityDone = "false",
                    ReviewJson = string.Empty,
                    IsReviewDone = "false",
                    IsReadLater = IsReadLater.ToLower(),
                    ReadLaterOn = Boolean.Parse(IsReadLater) ? DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) : string.Empty,
                    LastDateAccessed = string.Empty,
                    ReviewCompletedOn = string.Empty,
                    ActivityCompletedOn = string.Empty,
                    BookCompletedOn = string.Empty,
                    Devices = new Devices
                    {
                        DeviceId = DeviceId
                    },
                    Bookmark = new Bookmark
                    {
                        CurrentPage = string.Empty,
                        ReadingTime = string.Empty,
                        ReviewTime = string.Empty,
                        ActivityTime = string.Empty
                    },
                };
                return _book;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool UpdateUserDetails(UpdateMethod Update_Method)
        {
            try
            {
                if (AppData.UserProgress != null)
                {
                    string temp = string.Empty;
                    switch (Update_Method)
                    {
                        case UpdateMethod.Browsing:
                            if (AppData.UserProgress.BrowsingProgress != null && AppData.UserProgress.BrowsingProgress.Progress.Count > 0)
                            {
                                temp = MyWebRequest.PostRequest("syncuserbrowsingtime", null, AppData.UserProgress, null);
                            }
                            if (AppData.UserProgress.BrowsingProgress != null)
                            {
                                AppData.UserProgress.BrowsingProgress.Progress.Clear();
                            }
                            break;
                        case UpdateMethod.Books:
                            if (AppData.UserProgress.UserProgressBooks != null && AppData.UserProgress.UserProgressBooks.UserProgressBook.Count > 0)
                            {
                                temp = MyWebRequest.PostRequest("syncbookdata", null, AppData.UserProgress, null);
                            }
                            if (AppData.UserProgress.UserProgressBooks != null)
                            {
                                AppData.UserProgress.UserProgressBooks.UserProgressBook.Clear();
                            }
                            break;
                        case UpdateMethod.Total:
                            if ((AppData.UserProgress.BrowsingProgress != null && AppData.UserProgress.BrowsingProgress.Progress.Count > 0) || (AppData.UserProgress.UserProgressBooks != null && AppData.UserProgress.UserProgressBooks.UserProgressBook.Count > 0))
                            {
                                if (AppData.UserProgress.UserProgressBooks != null && AppData.UserProgress.UserProgressBooks.UserProgressBook.Count > 0)
                                {
                                    temp = MyWebRequest.PostRequest("syncuserdata", null, AppData.UserProgress, null);
                                }
                                else
                                {
                                    temp = MyWebRequest.PostRequest("syncuserbrowsingtime", null, AppData.UserProgress, null);
                                }
                            }
                            if (AppData.UserProgress.BrowsingProgress != null)
                            {
                                AppData.UserProgress.BrowsingProgress.Progress.Clear();
                            }
                            if (AppData.UserProgress.UserProgressBooks != null)
                            {
                                AppData.UserProgress.UserProgressBooks.UserProgressBook.Clear();
                            }
                            break;
                        case UpdateMethod.New:
                            temp = MyWebRequest.PostRequest("syncuserdata", null, AppData.UserProgress, null);
                            if (AppData.UserProgress.BrowsingProgress != null)
                            {
                                AppData.UserProgress.BrowsingProgress.Progress.Clear();
                            }
                            if (AppData.UserProgress.UserProgressBooks != null)
                            {
                                AppData.UserProgress.UserProgressBooks.UserProgressBook.Clear();
                            }
                            break;
                        default:

                            if ((AppData.UserProgress.BrowsingProgress != null && AppData.UserProgress.BrowsingProgress.Progress.Count > 0) || (AppData.UserProgress.UserProgressBooks != null && AppData.UserProgress.UserProgressBooks.UserProgressBook.Count > 0))
                            {
                                if (AppData.UserProgress.UserProgressBooks != null && AppData.UserProgress.UserProgressBooks.UserProgressBook.Count > 0)
                                {
                                    temp = MyWebRequest.PostRequest("syncuserdata", null, AppData.UserProgress, null);
                                }
                                else
                                {
                                    temp = MyWebRequest.PostRequest("syncuserbrowsingtime", null, AppData.UserProgress, null);
                                }
                            }
                            if (AppData.UserProgress.BrowsingProgress != null)
                            {
                                AppData.UserProgress.BrowsingProgress.Progress.Clear();
                            }
                            if (AppData.UserProgress.UserProgressBooks != null)
                            {
                                AppData.UserProgress.UserProgressBooks.UserProgressBook.Clear();
                            }
                            break;
                    }

                    if (!string.IsNullOrEmpty(temp) && !temp.ToLower().Equals("null"))
                    {
                        UserSyncDetails bookxml = JsonConvert.DeserializeObject<UserSyncDetails>(temp);
                        if (!string.IsNullOrEmpty(bookxml.UserDetails?.Trim()))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(UserDetails));
                            UserDetails details = (UserDetails)serializer.Deserialize(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(bookxml.UserDetails.ToString()))));
                            AppData.UserDetails = details != null ? details : AppData.UserDetails;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void AddUserSyncBook(string BookId)
        {
            try
            {
                if (AppData.UserProgress != null)
                {
                    UserProgressBook _bookProgress = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                    if (_bookProgress == null)
                    {
                        if (AppData.UserDetails != null)
                        {
                            UserBook _book = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault();
                            if (AppData.UserProgress.UserProgressBooks.UserProgressBook == null)
                            {
                                AppData.UserProgress.UserProgressBooks.UserProgressBook = new List<UserProgressBook>();
                            }
                            if (_book != null)
                            {
                                AppData.UserProgress.UserProgressBooks.UserProgressBook.Add(new UserProgressBook
                                {
                                    BookId = BookId,
                                    IsRead = _book.IsRead.ToLower(),
                                    IsReadLater = _book.IsReadLater.ToLower(),
                                    ReadLaterOn = string.IsNullOrEmpty(_book.ReadLaterOn) ? "" : DateTime.Parse(_book.ReadLaterOn).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                    LastDateAccessed = string.IsNullOrEmpty(_book.LastDateAccessed) ? "" : DateTime.Parse(_book.LastDateAccessed).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                    BookCompletedOn = string.IsNullOrEmpty(_book.BookCompletedOn) ? "" : DateTime.Parse(_book.BookCompletedOn).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                    CurrentPage = _book.Bookmark.CurrentPage,
                                    Activity = new Activity
                                    {
                                        ActivityCompletedOn = string.IsNullOrEmpty(_book.ActivityCompletedOn) ? "" : DateTime.Parse(_book.ActivityCompletedOn).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                        ActivityJson = _book.ActivityJson,
                                        ActivityProgress = new List<ActivityProgress>(),
                                        IsActivityDone = _book.IsActivityDone.ToLower()
                                    },
                                    BookReview = new BookReview
                                    {
                                        IsReviewDone = _book.IsReviewDone.ToLower(),
                                        ReviewCompletedOn = string.IsNullOrEmpty(_book.ReviewCompletedOn) ? "" : DateTime.Parse(_book.ReviewCompletedOn).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                        ReviewJson = _book.ReviewJson,
                                        ReviewProgress = new List<ReviewProgress>()
                                    },
                                    RatingLog = new RatingLog
                                    {
                                        Log = new List<Log>()
                                    },
                                    ReadingProgress = new ReadingProgress
                                    {
                                        Pages = new Pages()
                                    }
                                });
                            }
                            else
                            {
                                AppData.UserProgress.UserProgressBooks.UserProgressBook.Add(new UserProgressBook
                                {
                                    BookId = BookId,
                                    IsRead = string.Empty,
                                    IsReadLater = string.Empty,
                                    ReadLaterOn = string.Empty,
                                    LastDateAccessed = string.Empty,
                                    BookCompletedOn = string.Empty,
                                    CurrentPage = string.Empty,
                                    Activity = new Activity
                                    {
                                        ActivityCompletedOn = string.Empty,
                                        ActivityJson = string.Empty,
                                        ActivityProgress = new List<ActivityProgress>(),
                                        IsActivityDone = string.Empty
                                    },
                                    BookReview = new BookReview
                                    {
                                        IsReviewDone = string.Empty,
                                        ReviewCompletedOn = string.Empty,
                                        ReviewJson = string.Empty,
                                        ReviewProgress = new List<ReviewProgress>()
                                    },
                                    RatingLog = new RatingLog
                                    {
                                        Log = new List<Log>()
                                    },
                                    ReadingProgress = new ReadingProgress
                                    {
                                        Pages = new Pages()
                                    }
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void UpdateUserBookStatus()
        {
            try
            {
                if (AppData.UserDetails != null)
                {
                    int TotalBookRead = AppData.UserDetails.UserBooks.UserBook.Where(x => Boolean.Parse(string.IsNullOrEmpty(x.IsRead?.Trim()) ? "false" : x.IsRead.Trim()) == true).ToList().Count;
                    int TotalActivitiesCompleted = AppData.UserDetails.UserBooks.UserBook.Where(x => Boolean.Parse(string.IsNullOrEmpty(x.IsActivityDone?.Trim()) ? "false" : x.IsActivityDone?.Trim()) == true).ToList().Count;
                    int TotalBookRated = AppData.UserDetails.UserBooks.UserBook.Where(x => Int32.Parse(string.IsNullOrEmpty(x.Rating?.Trim()) ? "0" : x.Rating?.Trim()) != 0).ToList().Count;
                    TimeSpan TotalHourSpentOnReading = new TimeSpan();
                    foreach (UserBook book in AppData.UserDetails.UserBooks.UserBook)
                    {
                        if (!string.IsNullOrEmpty(book.Bookmark.ReadingTime?.Trim()))
                        {
                            string[] Time = book.Bookmark.ReadingTime?.Trim().Split(':');
                            if (Time.Length == 1)
                            {
                                int minutes = Int32.Parse(Time[0]);
                                TotalHourSpentOnReading = TotalHourSpentOnReading.Add(new TimeSpan((int)minutes / 60, (int)minutes % 60, 0));
                            }
                            if (Time.Length == 3)
                            {
                                TotalHourSpentOnReading = TotalHourSpentOnReading.Add(new TimeSpan(Convert.ToInt32(Time[0]), Convert.ToInt32(Time[1]), Convert.ToInt32(Time[2])));
                            }
                        }
                    }
                    AppData.UserDetails.TotalBookRead = TotalBookRead.ToString();
                    AppData.UserDetails.TotalBookRated = TotalBookRated.ToString();
                    AppData.UserDetails.TotalActivitiesCompleted = TotalActivitiesCompleted.ToString();
                    AppData.UserDetails.TotalHourSpentOnReading = Convert.ToInt32(TotalHourSpentOnReading.TotalMinutes).ToString();
                }
            }
            catch (Exception ex)
            {

            }
        }

        //public static bool StartDownloading(DownloadFile DownloadFile)
        //{
        //    try
        //    {
        //        Task[] tasks = new Task[DownloadFile.BookFiles.BookFile.Count];
        //        int i = 0;
        //        foreach (BookFile BookFile in DownloadFile.BookFiles.BookFile)
        //        {
        //            //BookFile.FileURL = @"https://kitaablet.blob.core.windows.net/kitaabletcn/temp/" + BookFile.FileName;
        //            if (!string.IsNullOrEmpty(BookFile.FileURL.Trim()))
        //            {
        //                tasks[i] = Task.Factory.StartNew(() => {
        //                    try
        //                    {
        //                        using (HttpClient client = new HttpClient())
        //                        {
        //                            using (HttpResponseMessage response = client.GetAsync(BookFile.FileURL.Trim()).Result)
        //                            {
        //                                if (response.IsSuccessStatusCode)
        //                                {
        //                                    using (HttpContent requestResponse = response.Content)
        //                                    {
        //                                        string result = requestResponse.ReadAsStringAsync().Result;
        //                                        if (result != null && !result.ToLower().Equals("null"))
        //                                        {
        //                                            result = result.Substring(1, result.Length - 2);
        //                                            using (HttpClient fileClient = new HttpClient())
        //                                            {
        //                                                using (HttpResponseMessage fileResponse = fileClient.GetAsync(result, HttpCompletionOption.ResponseHeadersRead).Result)
        //                                                {
        //                                                    using (Stream streamToReadFrom = fileResponse.Content.ReadAsStreamAsync().Result)
        //                                                    {
        //                                                        if (AppData.FileService.SaveBookDownload(DownloadFile.BookID, BookFile.FileName, streamToReadFrom))
        //                                                        {
        //                                                            BookFile.Status = true.ToString();
        //                                                        }
        //                                                    }
        //                                                }
        //                                            }
        //                                        }

        //                                        //using (Stream streamToReadFrom = requestResponse.ReadAsStreamAsync().Result)
        //                                        //{
        //                                        //    if (FileService.SaveBookDownload(DownloadFile.BookID, BookFile.FileName, streamToReadFrom))
        //                                        //    {
        //                                        //        BookFile.Status = true.ToString();
        //                                        //    }
        //                                        //}
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    catch (System.Exception ex)
        //                    {
        //                        BookFile.Status = false.ToString();
        //                    }
        //                });
        //            }
        //            i++;
        //        }
        //        Task.WaitAll(tasks);
        //        if (DownloadFile.BookFiles.BookFile.Where(x => bool.Parse(string.IsNullOrEmpty(x.Status) ? "false" : x.Status) == false).Count() == 0)
        //        {
        //            DownloadFile.IsDownloaded = true.ToString();
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //}

        public static bool IsBookDownloaded(string BookId)
        {
            try
            {
                if (AppData.BooksStatus != null)
                {
                    DownloadFile Download_File = AppData.BooksStatus.DownloadFile.Where(x => x.BookID.Equals(BookId))?.FirstOrDefault();
                    if (Download_File != null)
                    {
                        string filepath = Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + BookId);
                        if ((Utils.GetBookStatus(Download_File) == 4) && AppData.FileService.CheckDirectoryExistence(filepath))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static int GetBookStatus(DownloadFile DownloadFile)
        {
            try
            {
                if (string.IsNullOrEmpty(DownloadFile.IsDownloaded) ? false : Boolean.Parse(DownloadFile.IsDownloaded))
                {
                    if (string.IsNullOrEmpty(DownloadFile.IsUnZip) ? false : Boolean.Parse(DownloadFile.IsUnZip))
                    {
                        if (string.IsNullOrEmpty(DownloadFile.IsDecrypted) ? false : Boolean.Parse(DownloadFile.IsDecrypted))
                        {
                            if (string.IsNullOrEmpty(DownloadFile.IsEncrypted) ? false : Boolean.Parse(DownloadFile.IsEncrypted))
                            {
                                return 4;
                            }
                            else
                            {
                                return 3;
                            }
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static void SetBookStatus(DownloadFile DownloadFile)
        {
            try
            {
                if (AppData.BooksStatus != null)
                {
                    DownloadFile Download_File = AppData.BooksStatus.DownloadFile.Where(x => x.BookID.Equals(DownloadFile.BookID))?.FirstOrDefault();
                    if (Download_File != null)
                    {
                        Download_File.BookFiles = DownloadFile.BookFiles;
                        Download_File.BookID = DownloadFile.BookID;
                        Download_File.IsDecrypted = DownloadFile.IsDecrypted;
                        Download_File.IsDownloaded = DownloadFile.IsDownloaded;
                        Download_File.IsEncrypted = DownloadFile.IsEncrypted;
                        Download_File.IsProcessing = DownloadFile.IsProcessing;
                        Download_File.IsUnZip = DownloadFile.IsUnZip;
                    }
                    else
                    {
                        if (AppData.BooksStatus.DownloadFile == null)
                        {
                            AppData.BooksStatus.DownloadFile = new List<DownloadFile>();
                        }
                        AppData.BooksStatus.DownloadFile.Add(DownloadFile);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static MemoryStream DownloadFileAsync(string url, ActivityIndicatorLoader progress, CancellationToken token)
        {
            return Task.Run(async () =>
            {

                HttpClient client = new HttpClient();

                var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var total = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1L;
                var canReportProgress = total != -1 && progress != null;


                MemoryStream stream_m = new MemoryStream();

                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    var totalRead = 0L;
                    var buffer = new byte[8192];
                    var isMoreToRead = true;

                    do
                    {
                        token.ThrowIfCancellationRequested();

                        var read = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                        if (read == 0)
                        {
                            isMoreToRead = false;
                        }
                        else
                        {
                            //stream.CopyTo(stream_m);

                            var data = new byte[read];
                            buffer.ToList().CopyTo(0, data, 0, read);

                            // TODO: put here the code to write the file to disk
                            stream_m.Write(data, 0, data.Length);

                            totalRead += read;

                            if (canReportProgress)
                            {
                                progress.ProgressControl.Progress = ((totalRead * 1d) / (total * 1d) * 100) / 100;
                            }
                        }
                    } while (isMoreToRead);
                }

                return stream_m;

            }).Result;
        }

    }
}
