using FISE_Browser.Models;
using FISE_Browser.TWebClients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FISE_Browser.Controllers
{
    public class MainController : BaseController
    {
        private readonly TWebClient _webClient;
        public List<UserEvent> UserEvents;
        public List<int> NewEventID;

        public MainController()
        {
            _webClient = new TWebClient();
        }

        protected void getEvent()
        {
            if (Session["UserObject"] == null)
                CreateSession();
            User userobj = (User)Session["UserObject"];
            _webClient.UserId = userobj.UserId;
            List<UserEvent> UserEvent1 = _webClient.DownloadData<List<UserEvent>>("GetEvents", new { UserId = userobj.UserId });
            NewEventID = new List<int>();

            if (UserEvents != null)
            {
                foreach (UserEvent item in UserEvent1)
                {
                    bool flag = false;

                    foreach (UserEvent events in UserEvents)
                    {
                        if (events.EventId == item.EventId)
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        UserEvents.Add(item);
                    }
                }
            }
            else
            {
                UserEvents = UserEvent1;
            }

            if (UserEvents != null)
            {
                int count = UserEvents.Where(x => x.IsView == false).Count();
                foreach (UserEvent _event in UserEvents)
                {
                    if (!_event.IsView)
                    {
                        NewEventID.Add(_event.EventId);
                    }
                }
            }
        }

        public ActionResult Home()
        {
            BooksDetail booksobj = _webClient.DownloadData<BooksDetail>("getbookscatlog", null);
            if (Session["UserObject"] == null)
                CreateSession();
            User userobj = (User)Session["UserObject"];
            if (userobj != null)
            {
                _webClient.UserId = userobj.UserId;
                UserDetails result = (new TWebClient()).DownloadData<UserDetails>("getuserdata", new { UserId = userobj.UserId });
                userobj.AllBooks = booksobj;
                userobj.UserDetails = result;
                return View(userobj);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [ValidateInput(false)]
        public ActionResult Search(string query = "")
        {
            ViewBag.SearchString = query;
            BooksDetail booksobj = _webClient.DownloadData<BooksDetail>("getbookscatlog", null);
            if (Session["UserObject"] == null)
                CreateSession();
            User userobj = (User)Session["UserObject"];
            if (userobj != null)
            {
                return View(booksobj);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Setting()
        {
            if (Session["UserObject"] == null)
                CreateSession();
            User userobj = (User)Session["UserObject"];
            if (userobj != null)
            {
                return View();
            }
            else
                return RedirectToAction("Index", "Login");
        }

        public ActionResult Announcements()
        {
            if (Session["UserObject"] == null)
                CreateSession();
            User userobj = (User)Session["UserObject"];
            if (userobj != null)
            {
                getEvent();
                if (UserEvents != null)
                {
                    return View(UserEvents);
                }
                else
                {
                    return View(UserEvents);
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public ActionResult AnnouncementCount()
        {
            try
            {
                if (Session["eventCount"] == null)
                {
                    int count = 0;
                    getEvent();
                    foreach (UserEvent User_Event in UserEvents)
                    {
                        if (!User_Event.IsView)
                        {
                            count++;
                        }
                    }
                    string eventCount = count != 0 ? count.ToString() : string.Empty;
                    Session["eventCount"] = eventCount;
                    return Json(new { Count = eventCount }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Count = Session["eventCount"] as string }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex) { return Json(new { Count = string.Empty }, JsonRequestBehavior.AllowGet); }
        }

        public ActionResult PostQuestion(string ContactUsQuestion)
        {
            if (Session["UserObject"] == null)
                CreateSession();
            User userobj = (User)Session["UserObject"];
            bool resultContent = false;
            if (userobj != null)
            {
                _webClient.UserId = userobj.UserId;
                resultContent = _webClient.UploadData<bool>("createhelpitem", new { UserId = userobj.UserId, UserMessage = ContactUsQuestion });
            }
            return Json(new { Result = resultContent }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateEventStatus(UserEvent events)
        {
            bool status = false;
            if (Session["UserObject"] == null)
                CreateSession();
            User userobj = (User)Session["UserObject"];
            if (userobj != null)
            {
                getEvent();
                if (UserEvents != null)
                {
                    string viewEvents = string.Empty;
                    if (events.IsView && NewEventID.Contains(events.EventId))
                    {
                        viewEvents += events.EventId;
                    }
                    viewEvents = viewEvents.TrimEnd(',');

                    if (!string.IsNullOrEmpty(viewEvents))
                    {
                        status = (new TWebClient()).UploadData<bool>("UpdateViewEvents", new { UserId = userobj.UserId, EventIds = viewEvents });
                        if (status)
                        {
                            if (Session["eventCount"] != null && Session["eventCount"].ToString() != string.Empty)
                            {
                                int count = Convert.ToInt32(Session["eventCount"]);
                                string eventCount = count != 0 && count - 1 != 0 ? (count - 1).ToString() : string.Empty;
                                Session["eventCount"] = eventCount;
                            }
                        }
                    }

                }
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Index");
            }
        }

        [ChildActionOnly]
        public ActionResult GetUAC()
        {
            if (Session["UserObject"] == null)
                CreateSession();
            User userobj = (User)Session["UserObject"];
            UserActivityComponent uac = new UserActivityComponent();
            if (userobj != null)
            {
                _webClient.UserId = userobj.UserId;
                userobj.UserDetails = new UserDetails();
                UAC result = _webClient.DownloadData<UAC>("getuac", new { UserId = userobj.UserId });
                //UserDetails result = _webClient.DownloadData<UserDetails>("getuserdata", new { UserId = userobj.UserId });
                //BooksDetail booksobj =  ( new TWebClient()).DownloadData<BooksDetail>("getbookscatlog", null);

                //userobj.AllBooks = booksobj;
                //userobj.UserDetails = result;

                userobj.UserDetails.LastAccessedBookId = result.LastAccessedBookId;
                userobj.UserDetails.LastReadLaterBookId = result.LastReadLaterBookId;
                userobj.UserDetails.TotalActivitiesCompleted = result.TotalActivitiesCompleted;
                userobj.UserDetails.TotalBookRated = result.TotalBookRated;
                userobj.UserDetails.TotalBookRead = result.TotalBookRead;
                userobj.UserDetails.TotalHourSpentOnReading = result.TotalHourSpentOnReading;
                uac.imagePath = result.Thumbnail2;
                uac.IsLandscape = result.ViewMode == "Landscape" ? true : false;
                uac.hasActivity = result.HasActivity;
                uac.hasAnimation = result.HasAnimation;
                uac.hasReadAloud = result.HasReadAloud;
                uac.subsectionId = result.SubSectionId;
                uac.hasActivityDone = result.IsActivityDone;
                uac.ratingGiven = result.Rating > 0;
                uac.givenRating = result.Rating;
                uac.currentBookId = result.BookId;
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            UACViewModel uacmodel = new UACViewModel
            {
                ComponentData = uac,
                UserData = userobj
            };
            return PartialView("UserActivityComponent", uacmodel);
        }

        public JsonResult FocusOut()
        {
            if (Session["UserObject"] == null)
                CreateSession();

            if (Session["UserObject"] != null)
            {
                User userobj = (User)Session["UserObject"];
                if (Session["starttime"] != null)
                {
                    (new Helper.Helper()).UpdateBrowsing("timeout", userobj.SessionId, Convert.ToDateTime(Session["starttime"]), userobj.UserId);
                    Session["starttime"] = null;
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult FocusIn()
        {
            //if (Session["UserObject"] != null)
            // if (Session["starttime"] == null)
            if (Session["UserObject"] == null)
                CreateSession();
            if (Session["starttime"] == null)
                Session["starttime"] = DateTime.UtcNow;
            return Json(DateTime.UtcNow.ToString(), JsonRequestBehavior.AllowGet);
        }

    }
}