using FISE_API.Models;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;

namespace FISE_API.Services
{
    public class UserDataSyncService
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["FISE_APIConString"].ConnectionString;

        public bool MoveSyncDataToBackLog()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spMoveSyncDataToBackLog", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);

                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        int Status = (int)id.Value;
                        if (Status == 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return false;
            }
        }

        public List<UserSyncData> SearchUserSyncData()
        {
            var _UserSyncData = new List<UserSyncData>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetUserSyncData", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        _SqlDataAdapter.Fill(_DataSet);
                        if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                            {
                                UserSyncData _SyncData = new UserSyncData();
                                _SyncData.Id = int.Parse(_DataRow["Id"].ToString());
                                _SyncData.UserId = int.Parse(_DataRow["UserId"].ToString());
                                _SyncData.Data = _DataRow["Data"].ToString();
                                _SyncData.SyncType = _DataRow["SyncType"].ToString();
                                _SyncData.CreatedOn = DateTime.Parse(_DataRow["CreatedOn"].ToString());
                                _SyncData.IsStatsSynced = bool.Parse(_DataRow["IsStatsSynced"].ToString());
                                _UserSyncData.Add(_SyncData);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {

            }
            return _UserSyncData;
        }

        public void SyncUserData()
        {
            var queuedData = SearchUserSyncData();
            //SyncUserData
            string BrowsingProgressXml = "";
            double BrowsingTime = 0;
            string ReadingProgressXml = "";
            string ReadingLogXml = "";
            string ActivityProgressXml = "";
            string ReviewProgressXml = "";
            string RatingLogXml = "";
            string BooksXML = "";
            int dataID = 0;
            foreach (var data in queuedData)
            {
                //SyncUserData
                BrowsingProgressXml = "";
                BrowsingTime = 0;
                ReadingProgressXml = "";
                ReadingLogXml = "";
                ActivityProgressXml = "";
                ReviewProgressXml = "";
                RatingLogXml = "";
                BooksXML = "";
                dataID = data.Id;
                try
                {
                    if (!String.IsNullOrEmpty(data.Data))
                    {
                        UserProgressForSync UserSyncData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProgressForSync>(data.Data, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd hh:mm:ss tt" });
                        if (UserSyncData.BrowsingProgress != null && UserSyncData.BrowsingProgress.Progress.Count > 0)
                        {
                            var BrowsingProgress = ClaculateDateWiseProgress(UserSyncData.BrowsingProgress.Progress);
                            BrowsingProgressXml = GetProgressXml(BrowsingProgress);
                            if (!data.IsStatsSynced)
                            {
                                foreach (Progress bdata in UserSyncData.BrowsingProgress.Progress)
                                {
                                    BrowsingTime += Math.Round(bdata.EndTime.Subtract(bdata.StartTime).TotalSeconds);
                                }
                                if (BrowsingTime > 0)
                                    BrowsingTime = Math.Round(BrowsingTime / 60);//CALCULATE TOTAL MINS
                            }
                        }
                        if (UserSyncData.UserProgressBooks != null && UserSyncData.UserProgressBooks.UserProgressBook.Count > 0)
                        {
                            var books = XDocument.Parse("<Books></Books>");
                            foreach (UserProgressBook bdata in UserSyncData.UserProgressBooks.UserProgressBook)
                            {
                                if (bdata.ReadingProgress != null && bdata.ReadingProgress.Pages.Page != null && bdata.ReadingProgress.Pages.Page.Count > 0)
                                {
                                    var _ReadingProgress = bdata.ReadingProgress.Pages.Page.Select(x => new Progress { StartTime = x.StartTime, EndTime = x.EndTime }).ToList();
                                    var ReadingProgress = ClaculateDateWiseProgress(_ReadingProgress);
                                    ReadingProgressXml = GetProgressXml(ReadingProgress, bdata.BookId);

                                    var root = XDocument.Parse("<ReadingLogs></ReadingLogs>");
                                    var _ReadingLogXml = from c in bdata.ReadingProgress.Pages.Page
                                                         select new XElement("ReadingLog",
                                                        new XElement("PageNumber", c.PageNumber),
                                                        new XElement("StartTime", c.StartTime),
                                                        new XElement("EndTime", c.EndTime),
                                                        new XElement("BookId", bdata.BookId)
                                        );

                                    root.Root.Add(_ReadingLogXml);
                                    ReadingLogXml = root.ToString();
                                }
                                if (bdata.Activity.ActivityProgress != null && bdata.Activity.ActivityProgress.Count > 0)
                                {
                                    var _ActivityProgres = bdata.Activity.ActivityProgress.Select(x => new Progress { StartTime = x.StartTime, EndTime = x.EndTime }).ToList();
                                    var ActivityProgres = ClaculateDateWiseProgress(_ActivityProgres);
                                    ActivityProgressXml = GetProgressXml(ActivityProgres, bdata.BookId);
                                }
                                if (bdata.BookReview.ReviewProgress != null && bdata.BookReview.ReviewProgress.Count > 0)
                                {
                                    var _ReviewProgress = bdata.BookReview.ReviewProgress.Select(x => new Progress { StartTime = x.StartTime, EndTime = x.EndTime }).ToList();
                                    var ReviewProgress = ClaculateDateWiseProgress(_ReviewProgress);
                                    ReviewProgressXml = GetProgressXml(ReviewProgress, bdata.BookId);
                                }
                                if (bdata.RatingLog != null && bdata.RatingLog.Log != null && bdata.RatingLog.Log.Count > 0)
                                {
                                    var root = XDocument.Parse("<RatingLogs></RatingLogs>");
                                    var _RatingLogXml = from c in bdata.RatingLog.Log
                                                        select new XElement("RatingLog",
                                                        new XElement("Rating", c.Rating),
                                                        new XElement("RatedOn", c.RatedOn),
                                                        new XElement("BookId", bdata.BookId)
                                        );
                                    root.Root.Add(_RatingLogXml);
                                    RatingLogXml = root.ToString();
                                }
                                if (!data.IsStatsSynced)
                                {
                                    double ActivityTime = 0;
                                    double ReadingTime = 0;
                                    double ReviewTime = 0;
                                    foreach (ActivityProgress adata in bdata.Activity.ActivityProgress)
                                    {
                                        ActivityTime += Math.Round(adata.EndTime.Subtract(adata.StartTime).TotalSeconds);
                                    }
                                    if (ActivityTime > 0)
                                        ActivityTime = Math.Round(ActivityTime / 60);//CALCULATE TOTAL MINS

                                    List<Page> pages = bdata.ReadingProgress.Pages.Page.GroupBy(c => new { c.StartTime, c.EndTime }).Select(c => c.First()).ToList();
                                    foreach (Page rdata in pages)
                                    {
                                        ReadingTime += Math.Round(rdata.EndTime.Subtract(rdata.StartTime).TotalSeconds);
                                    }
                                    if (ReadingTime > 0)
                                        ReadingTime = Math.Round(ReadingTime / 60);//CALCULATE TOTAL MINS

                                    foreach (ReviewProgress rdata in bdata.BookReview.ReviewProgress)
                                    {
                                        ReviewTime += Math.Round(rdata.EndTime.Subtract(rdata.StartTime).TotalSeconds);
                                    }
                                    if (ReviewTime > 0)
                                        ReviewTime = Math.Round(ReviewTime / 60);//CALCULATE TOTAL MINS

                                    var Book = new XElement("Book",
                                       new XElement("BookId", bdata.BookId),
                                       new XElement("IsRead", bdata.IsRead),
                                       new XElement("IsReadLater", bdata.IsReadLater),
                                       new XElement("ReadLaterOn", bdata.ReadLaterOn),
                                       new XElement("LastDateAccessed", bdata.LastDateAccessed),
                                       new XElement("BookCompletedOn", bdata.BookCompletedOn),
                                       new XElement("CurrentPage", bdata.CurrentPage),
                                       new XElement("IsActivityDone", bdata.Activity.IsActivityDone),
                                       new XElement("ActivityCompletedOn", bdata.Activity.ActivityCompletedOn),
                                       new XElement("ActivityJson", bdata.Activity.ActivityJson),
                                       new XElement("ActivityTime", ActivityTime),
                                       new XElement("Rating", (bdata.RatingLog.Log != null && bdata.RatingLog.Log.Count > 0) ? bdata.RatingLog.Log.OrderByDescending(x => x.RatedOn).FirstOrDefault().Rating : 0),
                                       new XElement("RatedOn", (bdata.RatingLog.Log != null && bdata.RatingLog.Log.Count > 0) ? bdata.RatingLog.Log.OrderByDescending(x => x.RatedOn).FirstOrDefault().RatedOn : null),
                                       new XElement("ReadingTime", ReadingTime),
                                       new XElement("BookReadStartedOn", (bdata.ReadingProgress != null && bdata.ReadingProgress.Pages.Page.Count > 0) ? bdata.ReadingProgress.Pages.Page.OrderBy(x => x.StartTime).FirstOrDefault().StartTime.ToString() : null),
                                       new XElement("IsReviewDone", bdata.BookReview.IsReviewDone),
                                       new XElement("ReviewCompletedOn", bdata.BookReview.ReviewCompletedOn),
                                       new XElement("ReviewJson", bdata.BookReview.ReviewJson),
                                       new XElement("ReviewTime", ReviewTime)
                                       );
                                    books.Root.Add(Book);
                                }
                            }
                            BooksXML = books.ToString();
                            if (data.IsStatsSynced)
                            {
                                BooksXML = "";
                            }

                        }

                        using (SqlConnection con = new SqlConnection(connectionString))
                        {
                            using (SqlCommand command = new SqlCommand("spSyncUserDataByJob", con))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserSyncData.UserId;
                                command.Parameters.Add("@DeviceId", SqlDbType.Int).Value = UserSyncData.DeviceId;
                                command.Parameters.Add("@DataId", SqlDbType.Int).Value = dataID;
                                command.Parameters.Add("@BrowsingTime", SqlDbType.Int).Value = BrowsingTime;
                                command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = data.SyncType;
                                command.Parameters.Add("@IsStatsSynced", SqlDbType.Bit).Value = data.IsStatsSynced;
                                command.Parameters.Add("@BrowsingProgressXml", SqlDbType.Xml).Value = BrowsingProgressXml;
                                command.Parameters.Add("@ReadingProgressXml", SqlDbType.Xml).Value = ReadingProgressXml;
                                command.Parameters.Add("@ReadingLogXml", SqlDbType.Xml).Value = ReadingLogXml;
                                command.Parameters.Add("@ActivityProgressXml", SqlDbType.Xml).Value = ActivityProgressXml;
                                command.Parameters.Add("@ReviewProgressXml", SqlDbType.Xml).Value = ReviewProgressXml;
                                command.Parameters.Add("@RatingLogXml", SqlDbType.Xml).Value = RatingLogXml;
                                command.Parameters.Add("@BooksXML", SqlDbType.NVarChar).Value = BooksXML;
                                if (con.State == ConnectionState.Open)
                                    con.Close();
                                con.Open();
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }


        }
        private List<DatewiseMins> ClaculateDateWiseProgress(List<Progress> progress)
        {
            List<DatewiseMins> Dates = new List<DatewiseMins>();
            foreach (Progress _progress in progress)
            {
                DateTime start = _progress.StartTime;
                DateTime end = _progress.EndTime;
                var selectedDates = new List<DateTime>();
                for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
                {
                    selectedDates.Add(date);
                }
                if (selectedDates.Count > 0)
                {
                    double mins = 0;
                    string sdate = "";
                    if (selectedDates.Count == 1)
                    {
                        sdate = selectedDates[0].Date.ToString();
                        mins = Math.Round(end.Subtract(start).TotalMinutes);
                        Dates.Add(new DatewiseMins { Mins = mins, Date = sdate });
                    }
                    else
                    {
                        foreach (DateTime date in selectedDates)
                        {
                            sdate = date.Date.ToString();
                            if (date.Date == start.Date)
                            {
                                DateTime startend = new DateTime(start.Year, start.Month, start.Day, 23, 59, 59, 999);
                                mins = Math.Round(startend.Subtract(start).TotalMinutes);
                            }
                            else if (date.Date == end.Date)
                            {
                                DateTime endstart = new DateTime(end.Year, end.Month, end.Day, 0, 0, 0);
                                mins = Math.Round(end.Subtract(endstart).TotalMinutes);
                            }
                            else
                            {
                                mins = 24 * 60;
                            }

                            Dates.Add(new DatewiseMins { Mins = mins, Date = sdate });
                        }
                    }
                }
            }
            return Dates.GroupBy(g => g.Date).Select(
                g => new DatewiseMins
                {
                    Date = g.Key,
                    Mins = g.Sum(s => s.Mins)
                }).ToList();
        }
        private string GetProgressXml(List<DatewiseMins> progress, int BookId = 0)
        {
            var root = XDocument.Parse("<Progresses></Progresses>");
            var progressxml = from c in progress
                              select new XElement("Progress",
                              new XElement("Duration", c.Mins.ToString()),
                              new XElement("ProgressDate", c.Date.ToString()),
                              new XElement("BookId", BookId)
                );
            root.Root.Add(progressxml);
            return root.ToString();
        }
    }
    class DatewiseMins
    {
        public double Mins { get; set; }
        public string Date { get; set; }
    }
}
